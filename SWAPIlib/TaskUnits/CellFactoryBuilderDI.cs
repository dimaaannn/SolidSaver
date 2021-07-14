using NLog;
using SWAPIlib.Table;
using SWAPIlib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    public interface ICellFactoryBuilder
    {
        ITableAction Build();
        CellFactoryBuilderDI From(ModelPropertyNames propertyName);
        CellFactoryBuilderDI From(string text);
        CellFactoryBuilderDI FromReference(ICell cellReference);
        CellFactoryBuilderDI FromTextCopy(ICell cellReference);
        CellFactoryBuilderDI WithKey(string key);
        CellFactoryBuilderDI WithKey<T>(T en) where T : Enum;
        CellFactoryBuilderDI WithSettings(ITableAction tableAction);
        CellFactoryBuilderDI WithSettings(params ITable[] settingsTables);
        CellFactoryBuilderDI WithTarget(object target);
    }

    public class CellFactoryBuilderDI : ICellFactoryBuilder
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ITaskUnitFactory taskUnitFactory;

        private CellProviderBuilder factoryProviderBuilder;
        private IExtendedTable settingsTable;
        private ICellFactoryTemplate cellFactoryTemplate;

        public CellFactoryBuilderDI(ITaskUnitFactory taskUnitFactory, ICellFactoryTemplate cellFactoryTemplate)
        {
            this.taskUnitFactory = taskUnitFactory ?? throw new ArgumentNullException(nameof(taskUnitFactory));
            this.cellFactoryTemplate = cellFactoryTemplate ?? throw new ArgumentNullException(nameof(cellFactoryTemplate));

            factoryProviderBuilder = taskUnitFactory.CreateCellProviderBuilder();
            settingsTable = taskUnitFactory.CreateExtendedTable();
            logger.Trace("Instanced with {factoryTemplate}", cellFactoryTemplate);
        }

        private CellFactoryBuilderDI Clear()
        {
            logger.Trace("Clear factory");
            factoryProviderBuilder = taskUnitFactory.CreateCellProviderBuilder();
            settingsTable = taskUnitFactory.CreateExtendedTable();
            return this;
        }

        public CellFactoryBuilderDI From(string text)
        {
            logger.Trace("from text {name}", text);
            factoryProviderBuilder.CellGetter = CellFactoryBuilderDI.BuildFromText(text);
            return this;
        }
        public CellFactoryBuilderDI From(ModelPropertyNames propertyName)
        {
            logger.Trace("from template {name}", propertyName.ToString());
            ParseTemplate(propertyName);
            return this;
        }


        public CellFactoryBuilderDI FromTextCopy(ICell cellReference)
        {
            factoryProviderBuilder.CellGetter = BuildFromText(textValue: cellReference.Text);
            return this;
        }

        public CellFactoryBuilderDI FromReference(ICell cellReference)
        {
            logger.Trace("from reference {ref}", cellReference);
            factoryProviderBuilder.CellGetter = (refTable, settings) => cellReference;
            return this;
        }

        public CellFactoryBuilderDI WithKey(string key)
        {
            factoryProviderBuilder.Key = key;
            return this;
        }

        public CellFactoryBuilderDI WithKey<T>(T en) where T : Enum
        {
            return WithKey(en.ToString());
        }

        public CellFactoryBuilderDI WithTarget(object target)
        {
            var targetWrapperFactory = taskUnitFactory.CreatePartWrapperFactory();

            settingsTable.Target = targetWrapperFactory.GetObjectWrapper(target);
            return this;
        }

        public CellFactoryBuilderDI WithSettings(params ITable[] settingsTables)
        {
            logger.Debug("Add {count} setting table", settingsTables.Length);
            foreach (var table in settingsTables)
            {
                table.CopyTo(settingsTable, true);
            }
            return this;
        }

        public CellFactoryBuilderDI WithSettings(ITableAction tableAction)
        {
            ITable tempTable = taskUnitFactory.CreateTable();
            tableAction.Proceed(ref tempTable);
            tempTable.CopyTo(settingsTable, true);
            return this;
        }

        public ITableAction Build()
        {
            logger.Debug("build new factory {name}", factoryProviderBuilder.Name);
            ICellFactoryProvider cellFactoryProvider = factoryProviderBuilder.Build();
            ITableAction cellFactory = taskUnitFactory.CreateCellActionFactory(cellFactoryProvider);

            if (settingsTable.Count() > 0)
            {
                cellFactory.OptionalSettings = settingsTable;
            }


            Clear();
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

        private static CellGetterDelegate BuildFromText(string textValue)
        {
            return (refTable, settings) =>
            {
                return new TextCell(textValue);
            };
        }

    }

    public static class TableBuilderExtension
    {
        public static void AddTo(this ITableAction factory, IActionList tableBuilder)
        {
            tableBuilder.Add(factory);
        }
    }
}
