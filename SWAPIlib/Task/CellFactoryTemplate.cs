using SWAPIlib.Table;
using SWAPIlib.Table.Prop;
using SWAPIlib.Table.SWProp;
using SWAPIlib.Task.SWTask;
using System.Collections.Generic;

namespace SWAPIlib.Task
{
    /// <summary>
    /// Заготовленные шаблоны свойств для фабрики
    /// </summary>
    public interface ICellFactoryTemplate
    {
        ICellFactoryProvider GetCellProvider(ModelPropertyNames name);
    }

    public class CellFactoryTemplate : ICellFactoryTemplate
    {

        public ICellFactoryProvider GetCellProvider(ModelPropertyNames name)
        {
            switch (name)
            {
                case ModelPropertyNames.WorkFolder:
                    return WorkFolder();
                case ModelPropertyNames.ActiveConfigName:
                    return ActiveConfigName();
                case ModelPropertyNames.UserProperty:
                    return UserProperty();
                case ModelPropertyNames.FileName:
                    return FilePath();
                case ModelPropertyNames.TextBuilder:
                    return TextBuilder();
                case ModelPropertyNames.SaveSheetMetal:
                    return SaveSheetMetal();
                default:
                    break;
            }
            return null;
        }
        private static ICellFactoryProvider SaveSheetMetal()
        {
            return new CellFactoryProvider
            {
                Name = "Сохранить развёртку листового металла",
                Key = ModelPropertyNames.SaveSheetMetal.ToString(),
                CheckTable = SaveSheetMetalTask.CheckTargetType,
                Requirements = new HashSet<ModelEntities> { ModelEntities.FilePath },
                GetCell = (table, settings) =>
                    new SaveSheetMetalTask(table as ITargetTable) { Settings = settings }
            };
        }


        private static ICellFactoryProvider TextBuilder()
        {
            return new CellFactoryProvider
            {
                Name = "Составной текст",
                Key = ModelEntities.None.ToString(),
                GetCell = (table, settings) =>
                    new TextBuilderCell(table) { Settings = settings },
                CheckTable = TextBuilderCell.CheckTargetType,
                Requirements = new HashSet<ModelEntities> { ModelEntities.TextBuilderSettings },
                OverrideKey = true
            };
        }

        private static ICellFactoryProvider WorkFolder()
        {
            return new CellFactoryProvider(key: ModelEntities.Folder.ToString(),
                (refTable, settings) => new WorkFolderCell())
            {
                Name = "Рабочая папка",
                OverrideKey = false
            };
        }

        private static ICellFactoryProvider UserProperty()
        {
            var ret = new CellFactoryProvider()
            {
                Name = "Свойство пользователя",
                Key = ModelEntities.UserProperty.ToString(),
                GetCell = (table, settings) =>
                    new UserPropertyCell(table as ITargetTable) { Settings = settings },
                CheckTable = ActiveConfigNameCell.CheckTargetType,
                Requirements = new HashSet<ModelEntities>() { ModelEntities.ConfigName, ModelEntities.UserPropertyName }
            };
            return ret;
        }

        private static ICellFactoryProvider ActiveConfigName()
        {
            var ret = new CellFactoryProvider()
            {
                Name = "Имя активной конфигурации",
                Key = ModelEntities.ConfigName.ToString(),
                GetCell = (table, settings) =>
                    new ActiveConfigNameCell(table as ITargetTable) { Settings = settings }
                ,
                CheckTable = ActiveConfigNameCell.CheckTargetType
            };
            return ret;
        }

        private static ICellFactoryProvider FilePath()
        {
            return new CellFactoryProvider
            {
                Key = ModelEntities.FileName.ToString(),
                CheckTable = FilePathCell.CheckTargetType,
                Name = "Путь к файлу",
                GetCell = (refTable, settings) =>
                    new FilePathCell(refTable: refTable as ITargetTable)
            };
        }

        private static bool TryGetTarget(ITable table, out object obj)
        {
            bool ret = false;
            obj = null;
            if (table is ITargetTable tTable)
            {
                obj = tTable.GetTarget();
                ret = true;
            }

            return ret;
        }
    }
}
