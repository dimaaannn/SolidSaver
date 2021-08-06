using Ninject;

namespace SWAPIlib.TaskCollection
{
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
