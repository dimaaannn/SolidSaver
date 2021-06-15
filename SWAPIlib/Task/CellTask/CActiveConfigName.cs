using SWAPIlib.Table;
using SWAPIlib.Table.SWProp;
using System;

namespace SWAPIlib.Task.CellTask
{
    public class CActiveConfigName : CellTaskBase
    {
        public override string Name => "ActiveConfigName";

        public override CellLog Proceed(ref ICell cell, ITable settings)
        {
            var ret = Logger.Log<CActiveConfigName> (cell, settings);
            if (settings is ITargetTable tTable)
            {
                cell = new UserPropertyCell(tTable);
                ret.Action = CellAction.Processed;
            }
            else
                ret.Action = CellAction.Failed;

            return ret;
        }
    }
}
