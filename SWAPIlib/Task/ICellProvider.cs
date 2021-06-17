using SolidWorks.Interop.sldworks;
using SWAPIlib.Table;
using SWAPIlib.Table.SWProp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Task
{
    public interface ICellProvider : ITableChecker
    {
        string Name { get; }
        CellGetterDelegate GetCell { get; }
        HashSet<ModelEntities> Requirements { get; }
    }

    public class CellProvider : ICellProvider
    {
        public string Name { get; set; }

        public CellGetterDelegate GetCell { get; set; }

        public CheckTableDelegate CheckTable { get; set; }

        public HashSet<ModelEntities> Requirements { get; set; } = new HashSet<ModelEntities>();

    }



    public class CellProviderTemplate
    {
      
        public ICellProvider GetCellProvider(ModelPropertyNames name)
        {
            switch (name)
            {
                case ModelPropertyNames.ActiveConfigName:
                    return ActiveConfigName();
                case ModelPropertyNames.UserProperty:
                    return UserProperty();
                default:
                    break;
            }
            return null;
        }

        private static ICellProvider UserProperty()
        {
            var ret = new CellProvider()
            {
                Name = ModelPropertyNames.UserProperty.ToString(),
                CheckTable = ActiveConfigNameCell.CheckTargetType,
                GetCell = (table, settings) =>
                    new UserPropertyCell(table as ITargetTable) { Settings = settings },
                Requirements = new HashSet<ModelEntities>() { ModelEntities.ConfigName, ModelEntities.UserPropertyName }
            };
            return ret;
        }

        private static ICellProvider ActiveConfigName()
        {
            var ret = new CellProvider()
            {
                Name = ModelPropertyNames.ActiveConfigName.ToString(),
                CheckTable = ActiveConfigNameCell.CheckTargetType
                , GetCell = (table, settings) =>
                {
                    var newCell = new ActiveConfigNameCell(table as ITargetTable);
                    newCell.Settings = settings;
                    return newCell;

                }
            };
            return ret;
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
