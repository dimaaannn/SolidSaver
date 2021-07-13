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
