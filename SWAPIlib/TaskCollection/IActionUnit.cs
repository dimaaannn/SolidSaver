using Ninject;
using SWAPIlib.BaseTypes;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{
    /// <summary>
    /// Информация для пользователя
    /// </summary>
    public interface IUserInfo
    {
        string Name { get; }
        string InfoText { get; }
        string NextButtonText { get; }
    }

    /// <summary>
    /// Обработчик задач для таблиц
    /// </summary>
    public interface IActionUnit : IObservable<IActionUnitResult>
    {
        bool IsCanExecute { get; }
        IUserInfo UserInfo { get; }
        IActionUnit NextAction { get; }
        void Run(IExtendedTable table);
    }

    public class ActionUnit : IActionUnit
    {
        private readonly Subject<IActionUnitResult> tableProcessedSubject = new Subject<IActionUnitResult>();
        public bool IsCanExecute => ActionList?.Count() > 0;
        public IActionList ActionList { get; set; }
        public IUserInfo UserInfo { get; set; }
        public IActionUnit NextAction { get; set; }
        public IDisposable Subscribe(IObserver<IActionUnitResult> observer) => tableProcessedSubject.Subscribe(observer);

        public void Run(IExtendedTable table)
        {
            var result = ResultGenerator(table, ActionList.Proceed(table));
            tableProcessedSubject.OnNext(result);
        }


        private IActionUnitResult ResultGenerator(IExtendedTable table, TableLog tableLog)
        {
            ActionUnitStatus status = tableLog.Status != LogStatus.Failed ? ActionUnitStatus.Completed : ActionUnitStatus.Failed;
            return new ActionUnitResult {
                ActionUnit = this,
                TargetTable = table, 
                ActionLog = tableLog };
        }
    }

    public enum ActionUnitStatus
    {
        None,
        Failed,
        Passed,
        Completed
    }
    public interface IActionUnitResult
    {
        ActionUnit ActionUnit { get; }
        ActionUnitStatus Status { get; }
        IExtendedTable TargetTable { get; }
        TableLog ActionLog { get; }
    }

    public class ActionUnitResult : IActionUnitResult
    {
        public ActionUnit ActionUnit { get; set; }
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

    public abstract class ActionScerianoBase<T>
        where T : ITargetProvider
    {
        private readonly ITableCollection tableCollection;
        private readonly List<IActionUnit> actionUnits = new List<IActionUnit>();

        private int currentStep = 0;
        
        public ActionScerianoBase(ITableCollection tableCollection)
        {
            this.tableCollection = tableCollection;
        }
        public IActionUnit NextStep
        {
            get
            {
                if (currentStep < actionUnits.Count())
                    return actionUnits[currentStep];
                else return null;
            }
        }
        public bool Init(T partProvider)
        {
            return tableCollection.GetFromProvider(partProvider);
        }
    }


    /// <summary>
    /// Temp class for run test
    /// </summary>
    public class TableProvider //TODO refactor provider
    {
        public ITableCollection UserSelectedModels()
        {
            var tableCollection = Initialiser.kernel.Get<ITableCollection>();
            var partProvider = Initialiser.kernel.Get<SelectedModelProvider>();
            tableCollection.GetFromProvider(partProvider);
            return tableCollection;
        }
    }


}
