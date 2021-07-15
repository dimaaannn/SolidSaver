using SWAPIlib.Table;
using SWAPIlib.TaskCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits.Actions
{
    public class ModelActions
    {
        private readonly ITaskUnitFactory taskUnitFactory;
        private readonly ITaskServices taskServices;

        public ModelActions(ITaskUnitFactory taskUnitFactory, ITaskServices taskServices)
        {
            this.taskUnitFactory = taskUnitFactory;
            this.taskServices = taskServices;
        }


        /// <summary>
        /// Добавить ячейку ссылающуюся на пользовательское свойство модели SW
        /// </summary>
        /// <param name="targetList"></param>
        /// <param name="userPropertyName"></param>
        public ITableAction GetUserProperty(string userPropertyName, string key = null)
        {
            ICellFactoryBuilder factoryBuilder = taskUnitFactory.CreateCellFactoryBuilder();
            if (userPropertyName is null)
            {
                throw new ArgumentNullException(nameof(userPropertyName));
            }

            var settings = factoryBuilder
                .From(userPropertyName)
                .WithKey(Table.ModelEntities.UserPropertyName)
                .Build();

            var builder = factoryBuilder.From(Table.ModelPropertyNames.UserProperty)
                .WithKey(userPropertyName)
                .WithSettings(settings);
            if (string.IsNullOrEmpty(key))
                return builder.Build();
            else
                return builder
                    .WithKey(key)
                    .Build();
        }

        public ITableAction GetActiveConfigName(string key = null)
        {
            ICellFactoryBuilder factoryBuilder = 
                taskUnitFactory.CreateCellFactoryBuilder();
            var builder = factoryBuilder
                .From(Table.ModelPropertyNames.ActiveConfigName);
            if (string.IsNullOrEmpty(key))
                return builder.Build();
            else
                return builder
                    .WithKey(key)
                    .Build();
        }

        public ITableAction GetFileName(string key = null)
        {
            ICellFactoryBuilder factoryBuilder =
                taskUnitFactory.CreateCellFactoryBuilder();
            var builder = factoryBuilder.From(Table.ModelPropertyNames.FileName);

            if (string.IsNullOrEmpty(key))
                return builder.Build();
            else
                return builder
                    .WithKey(key)
                    .Build();
        }

        /// <summary>
        /// Добавить в список действий сохранение листового материала
        /// </summary>
        /// <param name="targetList">Список действий</param>
        /// <param name="getsavingFilePath">Конструктор имени сохранения</param>
        /// <param name="key">Ключ добавления</param>
        public ITableAction GetSaveSheetMetal(Func<ITable, string> getsavingFilePath, string key = null)
        {
            const string filePathKey = "FilePath";

            ICellFactoryBuilder factoryBuilder =
                taskUnitFactory.CreateCellFactoryBuilder();

            ITableAction settings = GetTextBuilder(filePathKey, getsavingFilePath);

            var saveSheetMetalBuilder = factoryBuilder
                    .From(Table.ModelPropertyNames.SaveSheetMetal)
                    .WithSettings(settings);

            if (string.IsNullOrEmpty(key) == false)
                saveSheetMetalBuilder.WithKey(key);

            return saveSheetMetalBuilder.Build();
        }

        public ITableAction GetTextBuilder(string key, Func<ITable, string> getTextFunc)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            if (getTextFunc is null)
                throw new ArgumentNullException(nameof(getTextFunc));

            ICellFactoryBuilder factoryBuilder =
                taskUnitFactory.CreateCellFactoryBuilder();

            ICell textBuilderSettingsGetFileName =
                Table.Prop.TextBuilderCell.BuildSettings(getTextFunc);

            ITableAction createTextBuilderSettings = factoryBuilder
                .FromReference(textBuilderSettingsGetFileName)
                .WithKey(Table.Prop.TextBuilderCell.SETTINGS_KEY)
                .Build();

            return factoryBuilder.From(Table.ModelPropertyNames.TextBuilder)
                .WithKey(key)
                .WithSettings(createTextBuilderSettings)
                .Build();
        }

    }

}
