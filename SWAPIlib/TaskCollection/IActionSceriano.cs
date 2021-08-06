using Ninject;
using SWAPIlib.BaseTypes;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{



    public class ActionManager : IDisposable
    {
        public IActionUnit CurrentStep { get; private set; }
        IEnumerator<IActionUnit> actionUnitsEnumerator;
        public void Begin(ActionDataProvider actionDataProvider)
        {
            actionUnitsEnumerator = actionDataProvider.ActionUnits.GetEnumerator();
            TakeNextStep();
        }

        private void TakeNextStep()
        {
            if (actionUnitsEnumerator.MoveNext())
            {
                CurrentStep = actionUnitsEnumerator.Current;
            }
            else
            {
                CurrentStep = null;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void GetNextStep()
        {

        }
    }

    public interface IActionDataProvider
    {
        IEnumerable<IActionUnit> ActionUnits { get; }
        ITargetProvider TargetProvider { get;}
    }

    public class ActionDataProvider : IActionDataProvider
    {
        public List<IActionUnit> ActionUnitsList { get; }
        public ActionDataProvider()
        {
            ActionUnitsList = new List<IActionUnit>();
        }
        public ITargetProvider TargetProvider { get; set; }
        public IEnumerable<IActionUnit> ActionUnits => ActionUnitsList;



    }

    /// <summary>
    /// Temp class for run test
    /// </summary>
    public class TableProviderTemp //TODO refactor provider
    {
        public TestAction TestAction { get; }
        public TableProviderTemp()
        {
            //Initialiser.kernel.Bind<TestAction>().ToSelf().InSingletonScope();
            TestAction = Initialiser.kernel.Get<TestAction>();
        }
        public ITableCollection UserSelectedModels()
        {
            var taskServices = Initialiser.kernel.Get<ITaskServices>();
            var partProvider = taskServices.CreateSelectedModelProvider();

            var tableProvider = taskServices.CreateTableProvider(partProvider);

            return tableProvider.GetTable();
        }


        public IActionUnit GetActionUnit()
        {
            //Вернуть
            IActionUnit ret = TestAction.GetGlobalInfoUnit();

            return ret;

        }
    }


}
