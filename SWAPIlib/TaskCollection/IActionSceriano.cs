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

        public void Begin(ActionDataProvider actionDataProvider)
        {
            throw null;
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
        public ITableCollection UserSelectedModels()
        {
            var taskServices = Initialiser.kernel.Get<ITaskServices>();
            var partProvider = taskServices.CreateSelectedModelProvider();

            var tableProvider = taskServices.CreateTableProvider(partProvider);

            return tableProvider.GetTable();
        }

        public IActionList GetTestActionList()
        {
            var testClass = new TestAction(Initialiser.kernel);
            return testClass.GlobalModelOptions();
        }
    }


}
