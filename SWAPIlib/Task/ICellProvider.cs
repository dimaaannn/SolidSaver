using SolidWorks.Interop.sldworks;
using SWAPIlib.Table;
using SWAPIlib.Table.Prop;
using SWAPIlib.Table.SWProp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Task
{
    public interface ICellProvider 
    {
        CellGetterDelegate GetCell { get; }
    }

    public interface IRequirementKeys
    {
        HashSet<ModelEntities> Requirements { get; }
    }

    public interface ICellFactoryProvider : ICellProvider, ITableChecker, IRequirementKeys
    {
        string Name { get; set; }
        string Key { get; set; }
        bool OverrideKey { get; set; }
    }

    public class CellFactoryProvider : ICellFactoryProvider
    {
        private string name;

        public CellFactoryProvider() { }
        public CellFactoryProvider(string key, CellGetterDelegate cellGetter)
        {
            Key = key;
            GetCell = cellGetter;
        }
        public string Name { get => name ?? Key; set => name = value; }
        public string Key { get; set; } = "NoKey";
        public bool OverrideKey { get; set; } = false;
        public CellGetterDelegate GetCell { get; set; }
        public CheckTableDelegate CheckTable { get; set; } = (x, y) => true;
        public HashSet<ModelEntities> Requirements { get; set; } = new HashSet<ModelEntities>();
    }




    public class CellProviderTemplate
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
                default:
                    break;
            }
            return null;
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
                ,CheckTable = ActiveConfigNameCell.CheckTargetType
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
            if(table is ITargetTable tTable)
            {
                obj = tTable.GetTarget();
                ret = true;
            }
            
            return ret;
        }
    }
}
