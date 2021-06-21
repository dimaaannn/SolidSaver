using SWAPIlib.Table;
using SWAPIlib.Table.Prop;
using SWAPIlib.Table.SWProp;
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
                case ModelPropertyNames.FilePath:
                    return FilePath();
                case ModelPropertyNames.TextBuilder:
                    return TextBuilder();
                default:
                    break;
            }
            return null;
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
            return new CellFactoryProvider(
                ModelEntities.FileName.ToString(),
                (reftable, settings) => new FilePathCell(refTable: reftable as ITargetTable))
            {
                CheckTable = FilePathCell.CheckTargetType,
                Name = "Путь к файлу"
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
