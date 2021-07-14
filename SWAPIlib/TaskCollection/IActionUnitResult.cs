using SWAPIlib.Table;
using SWAPIlib.TaskUnits;

namespace SWAPIlib.TaskCollection
{
    public enum ActionUnitStatus
    {
        None,
        Failed,
        Passed,
        Completed
    }


    public interface IActionUnitResult
    {
        IActionUnit ActionUnit { get; }
        ActionUnitStatus Status { get; }
        IExtendedTable TargetTable { get; }
        TableLog ActionLog { get; }
    }

    public class ActionUnitResult : IActionUnitResult
    {
        public IActionUnit ActionUnit { get; set; }
        public ActionUnitStatus Status { get; set; }
        public IExtendedTable TargetTable { get; set; }
        public TableLog ActionLog { get; set; }

        public override string ToString()
        {
            return $"{ActionUnit?.UserInfo.Name}-\"{TargetTable?.Name}\": {Status}";
        }
    }

}
