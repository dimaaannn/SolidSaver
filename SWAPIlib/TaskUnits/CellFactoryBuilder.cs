using NLog;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.Utils;

namespace SWAPIlib.TaskUnits
{


    public class CellFactoryBuilder
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICellFactoryTemplate cellFactoryTemplate;

        private CellProviderBuilder factoryProviderBuilder;
        private IExtendedTable settingsTable;

        public CellFactoryBuilder(ICellFactoryTemplate cellFactoryTemplate)
        {
            this.cellFactoryTemplate = cellFactoryTemplate;
            factoryProviderBuilder = new CellProviderBuilder();
            settingsTable = new ExtendedTable();
            logger.Trace("Instanced with {factoryTemplate}", cellFactoryTemplate);
        }

        public static CellFactoryBuilder Create()
        {
            return new CellFactoryBuilder(new CellFactoryTemplate()); // TODO add DI singleton
        }

        public CellFactoryBuilder New(ModelPropertyNames propertyName)
        {
            logger.Trace("New from template {name}", propertyName.ToString());
            ParseTemplate(propertyName);
            return this;
        }

        public CellFactoryBuilder New(string text)
        {
            factoryProviderBuilder.CellGetter = BuildFromText(text);
            return this;
        }

        public CellFactoryBuilder TextCopy(ICell cellReference)
        {
            New(cellReference.Text);
            return this;
        }

        public CellFactoryBuilder Reference(ICell cellReference)
        {
            logger.Trace("New from reference {ref}", cellReference);
            factoryProviderBuilder.CellGetter = (refTable, settings) => cellReference;
            return this;
        }

        public CellFactoryBuilder WithKey(string key)
        {
            factoryProviderBuilder.Key = key;
            return this;
        }

        public CellFactoryBuilder WithTarget(object target)
        {
            settingsTable.Target = new TargetWrapper(target);
            return this;
        }

        public CellFactoryBuilder AddSettings(params ITable[] settingsTables)
        {
            logger.Debug("Add {count} setting table", settingsTables.Length);
            foreach (var table in settingsTables)
            {
                table.CopyTo(settingsTable, true);
            }
            return this;
        }

        public ICellFactory Build()
        {
            logger.Debug("build new factory {name}", factoryProviderBuilder.Name);
            ICellFactoryProvider cellFactoryProvider = factoryProviderBuilder.Build();
            ICellFactory cellFactory = new CellFactory(cellFactoryProvider);

            if (settingsTable.Count() > 0)
                cellFactory.GetSettingsDelegate = _ => settingsTable;

            return cellFactory;
        }

        private void ParseTemplate(ModelPropertyNames propertyName)
        {
            ICellFactoryProvider temp = cellFactoryTemplate.GetCellProvider(propertyName);

            factoryProviderBuilder.CellGetter = temp.GetCell;
            factoryProviderBuilder.CheckTable = temp.CheckTable;
            factoryProviderBuilder.Key = temp.Key;
            factoryProviderBuilder.Name = temp.Name;
        }

        private CellGetterDelegate BuildFromText(string textValue)
        {
            return (refTable, settings) =>
            {
                return new TextCell(textValue);
            };
        }

    }

}

namespace SWAPIlib.Utils
{
    using SWAPIlib.TaskUnits;

    /// <summary>
    /// Создаёт объект настройки фабрики по заданным параметрам
    /// </summary>
    public class CellProviderBuilder
    {
        public CellGetterDelegate CellGetter { get; set; } 
        public CheckTableDelegate CheckTable { get; set; }
        public string Key { get; set; }
        public bool OverrideKey { get; set; } = true;
        public string Name { get; set; }

        public ICellFactoryProvider Build()
        {
            if (CheckRequirements(this))
            {
                CheckTableDelegate alwaysTrue = (refTable, settings) => true;
                return new CellFactoryProvider
                {
                    GetCell = this.CellGetter,
                    Key = this.Key,
                    CheckTable = this.CheckTable ?? alwaysTrue,
                    Name = this.Name ?? "NoName",
                    OverrideKey = this.OverrideKey,
                };
            }
            else
                throw new AggregateException("invalid or null arguments");
        }

        private bool CheckRequirements(CellProviderBuilder builderSettings)
        {

            bool ret = builderSettings.CellGetter == null;
            ret &= string.IsNullOrEmpty(builderSettings.Key);
            return ret;
        }
    }

}
