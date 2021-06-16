using SolidWorks.Interop.sldworks;
using SWAPIlib.Table;
using SWAPIlib.Table.SWProp;
using System;
using System.Linq;

namespace SWAPIlib.Task
{
    public interface ICellProvider : ITableChecker
    {
        string Name { get; }
        CellGetterDelegate GetCell { get; }
    }

    public class CellProvider : ICellProvider
    {
        public string Name { get; set; }

        public CellGetterDelegate GetCell { get; set; }

        public CheckTableDelegate CheckTable { get; set; }

    }

    public enum ProviderName
    {
        ActiveConfigName
    }


    public class CellProviderTemplate
    {
      
        public ICellProvider GetCellProvider(ProviderName name)
        {
            switch (name)
            {
                case ProviderName.ActiveConfigName:
                    return ActiveConfigName();
                default:
                    break;
            }
            return null;
        }

        private static ICellProvider ActiveConfigName()
        {
            IPropertyCell temp = new ActiveConfigNameCell(null);
            var ret = new CellProvider()
            {
                Name = ProviderName.ActiveConfigName.ToString(),
                CheckTable = ActiveConfigNameCell.CheckTarget
                , GetCell = (table, settings) =>
                    new ActiveConfigNameCell(table as ITargetTable)
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
