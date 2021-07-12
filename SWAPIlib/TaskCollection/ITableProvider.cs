namespace SWAPIlib.TaskCollection
{
    public interface ITableProvider
    {
        bool IsCanGetTable { get; }
        ITableCollection GetTable();
    }

    public class TableProvider : ITableProvider
    {
        private readonly ITaskServices taskServices;
        private readonly ITargetProvider targetProvider;

        public TableProvider(ITaskServices taskServices, ITargetProvider targetProvider)
        {
            this.taskServices = taskServices;
            this.targetProvider = targetProvider;

        }
        public bool IsCanGetTable => targetProvider.IsCanGetTargets;

        public ITableCollection GetTable()
        {
            var ret = taskServices.CreateTableCollection();
            ret.GetFromProvider(targetProvider);
            return ret;
        }
    }


}
