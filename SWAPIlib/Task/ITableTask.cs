using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task
{
    public interface ITableTask : ITableAction, ITableChecker, IRequirementKeys
    {

    }

    public abstract class TableTaskBase : ITableTask
    {
        protected TableActionDelegate ProceedAction { get; set; }
        public string Name { get; set; }

        public CheckTableDelegate CheckTable { get; set; } = (reftable, settings) => true;
        public HashSet<ModelEntities> Requirements { get; set; } = new HashSet<ModelEntities>();

        public abstract TableLog Proceed(ref ITable refTable, ITable settings);

    }

    public class SaveSheetMetalTask : TableTaskBase
    {
        public SaveSheetMetalTask()
        {
            Requirements.Add(ModelEntities.FileName);
            Requirements.Add(ModelEntities.ConfigName);
            Requirements.Add(ModelEntities.IsSheetMetal);
        }

        public override TableLog Proceed(ref ITable refTable, ITable settings)
        {
            throw new NotImplementedException();
        }
    }


}
