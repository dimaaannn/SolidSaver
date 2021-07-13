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
        public ActionUnitStatus Status
        {
            get
            {
                ActionUnitStatus ret;
                switch (ActionLog.Status)
                {
                    case LogStatus.None:
                        ret = ActionUnitStatus.None;
                        break;
                    case LogStatus.Processed:
                        ret = ActionUnitStatus.Completed;
                        break;
                    case LogStatus.Passed:
                        ret = ActionUnitStatus.Passed;
                        break;
                    case LogStatus.Failed:
                        ret = ActionUnitStatus.Failed;
                        break;
                    default:
                        ret = ActionUnitStatus.None;
                        break;
                }
                return ret;
            }
        }
        public IExtendedTable TargetTable { get; set; }
        public TableLog ActionLog { get; set; }

        public override string ToString()
        {
            return $"{ActionUnit?.UserInfo.Name}-\"{TargetTable?.Name}\": {Status}";
        }
    }

}
