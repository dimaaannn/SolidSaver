using Ninject;
using SWAPIlib.Property.ModelProperty;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{
    public class TestAction
    {
        private readonly ITaskUnitFactory taskUnitFactory;
        private readonly ITaskServices taskServices;
        private readonly TaskUnits.Actions.ModelActions modelActions;

        public TestAction(ITaskUnitFactory taskUnitFactory, ITaskServices taskServices, TaskUnits.Actions.ModelActions modelActions)
        {
            this.taskUnitFactory = taskUnitFactory;
            this.taskServices = taskServices;
            this.modelActions = modelActions;
        }


        public const string FILE_NAME_KEY        = "FileName";
        public const string NOMINATION_KEY       = "Обозначение";
        public const string DESIGNATION_KEY      = "Наименование";
        public const string ACTIVE_CONFIG_KEY    = "ConfigName";


        public IActionUnit GetGlobalInfoUnit()
        {
            IActionList actionList = taskUnitFactory.CreateActionList();

            actionList.Add(modelActions.GetActiveConfigName("ActiveConfig"));
            actionList.Add(modelActions.GetFileName(FILE_NAME_KEY));
            

            //AddGlobalModelOptions(actionList);
            //AddUserProperty(actionList, NOMINATION_KEY);
            //AddUserProperty(actionList, DESIGNATION_KEY);

            IActionUnit ret = taskServices.CreateActionUnit(actionList);
            return ret;
        }

        public IEnumerable<IActionUnit> GetActionUnits()
        {
            IActionList step1 = taskUnitFactory.CreateActionList();

            step1.Add(modelActions.GetActiveConfigName(ACTIVE_CONFIG_KEY));
            step1.Add(modelActions.GetFileName(FILE_NAME_KEY));
            yield return taskServices.CreateActionUnit(step1);

            IActionList step2 = taskUnitFactory.CreateActionList();

            step2.Add(modelActions.GetUserProperty(NOMINATION_KEY));
            step2.Add(modelActions.GetUserProperty(DESIGNATION_KEY));
            yield return taskServices.CreateActionUnit(step2);
            
        }
    }
}
