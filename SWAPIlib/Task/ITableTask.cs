using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task
{
    public interface ITableTask : ITableAction, ITableChecker
    {

    }

    public class TableTask : ITableTask
    {
        public TableTask(TableActionDelegate action)
        {
            if (action == null)
                throw new NullReferenceException("TableTask: Null action ref");
            ProceedAction = action;
        }

        public string Name { get; set; }
        public CheckTableDelegate CheckTable { get; set; }
        protected TableActionDelegate ProceedAction { get; set; }

        public TableLog Proceed(ref ITable refTable, ITable settings) => 
            ProceedAction(refTable: ref refTable, settings: settings);
    }
}
