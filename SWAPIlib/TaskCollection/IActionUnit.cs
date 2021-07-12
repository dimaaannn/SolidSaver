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
        public ActionUnit()
        {
            tableProcessedSubject = new Subject<IActionUnitResult>();
        }
        private ISubject<IActionUnitResult> tableProcessedSubject;
        public bool IsCanExecute => ActionList?.Count() > 0;
        public IActionList ActionList { get; set; }
        public IUserInfo UserInfo { get; set; }
        public IActionUnit NextAction { get; set; }
        public IDisposable Subscribe(IObserver<IActionUnitResult> observer) =>  tableProcessedSubject.Subscribe(observer);

        public void Run(IExtendedTable table)
        {
            var result = ResultGenerator(table, ActionList.Proceed(table));
            tableProcessedSubject.OnNext(result);
        }


        private IActionUnitResult ResultGenerator(IExtendedTable table, TableLog tableLog)
        {
            ActionUnitStatus status = tableLog.Status != LogStatus.Failed ? ActionUnitStatus.Completed : ActionUnitStatus.Failed;
            return new ActionUnitResult
            {
                ActionUnit = this,
                TargetTable = table,
                ActionLog = tableLog
            };

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


    public interface IActionUnitSwitcher
    {
        bool MoveNext();
        IActionUnit Current { get; }
        IActionUnit Next { get; }
    }

    public class ActionUnitSwitcher : IActionUnitSwitcher
    {

        private int currentActionIndex = 0;

        public ActionUnitSwitcher(List<IActionUnit> actions)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public List<IActionUnit> Actions { get; set; }
        public IActionUnit Current => GetActionListByIndex(currentActionIndex);
        public IActionUnit Next => GetActionListByIndex(currentActionIndex + 1);
        public void Reset() => currentActionIndex = 0;

        public bool MoveNext()
        {
            return Actions.Count() > ++currentActionIndex;
        }

        private IActionUnit GetActionListByIndex(int index)
        {
            if (Actions?.Count() > index)
            {
                return Actions[index];
            }
            else 
                return null;
        }
    }



    public abstract class ActionScerianoBase
    {


        private int currentStepIndex = 0;
        private IDisposable currentStepSubscribe;
        private ScerianoData settings;
        private ITableCollection tableCollection;

        public ITableCollection TableCollection { get => tableCollection; }
        public IActionUnit CurrentStep
        {
            get => null;
            //{
            //    if (currentStepIndex < Settings?.ActionUnits.Count())
            //    {
            //        return Settings?.ActionUnits[currentStepIndex];
            //    }
            //    else return null;
            //}
        }

        public ActionScerianoBase.ScerianoData Settings 
        { 
            get => settings; 
            set => settings = value; 
        }

        public class ScerianoData
        {
            public ScerianoData(ITargetProvider targetProvider, List<IActionUnit> actionUnits)
            {
                TargetProvider = targetProvider ?? throw new ArgumentNullException(nameof(targetProvider));
            }
            public ITargetProvider TargetProvider { get; }
            public IActionUnitSwitcher ActionProvider { get; }
        }

        private void OnChangeSettings(ScerianoData settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            currentStepIndex = 0;
        }
        private void GetNextStep()
        {

        }
    }


    /// <summary>
    /// Temp class for run test
    /// </summary>
    public class TableProviderTemp //TODO refactor provider
    {
        public ITableCollection UserSelectedModels()
        {
            var taskServices = Initialiser.kernel.Get<ITaskServices>();
            var partProvider = taskServices.CreateSelectedModelProvider();

            var tableProvider = taskServices.CreateTableProvider(partProvider);

            return tableProvider.GetTable();
        }
    }


}
