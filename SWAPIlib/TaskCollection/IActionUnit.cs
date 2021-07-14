using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using System;
using System.Linq;
using System.Reactive.Subjects;

namespace SWAPIlib.TaskCollection
{

    /// <summary>
    /// Обработчик задач для таблиц
    /// </summary>
    public interface IActionUnit : IObservable<IActionUnitResult>
    {
        bool IsCanExecute { get; }
        IUserInfo UserInfo { get; }
        void Run(IExtendedTable table);
    }

    /// <summary>
    /// Информация для пользователя о задаче
    /// </summary>
    public interface IUserInfo
    {
        string Name { get; }
        string InfoText { get; }
        string NextButtonText { get; }
    }

    public class ActionUnit : IActionUnit
    {
        public ActionUnit()
        {
            tableProcessedSubject = new Subject<IActionUnitResult>();
        }

        public ActionUnit(IActionList actionList) : this()
        {
            ActionList = actionList;
        }

        private readonly ISubject<IActionUnitResult> tableProcessedSubject;
        public bool IsCanExecute => ActionList?.Count() > 0;
        public IActionList ActionList { get; set; }
        public IUserInfo UserInfo { get; set; }
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
                ActionLog = tableLog,
                Status = status
            };

        }
    }


}
