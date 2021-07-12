using SWAPIlib.BaseTypes;
using SWAPIlib.Table;

namespace SWAPIlib.TaskCollection
{
    public class TaskCollectionNinjectBinding : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<ISelectedModelProvider>().To<SelectedModelProvider>().InSingletonScope();
            Bind<ISelectedComponentProvider>().To<SelectedComponentProvider>().InSingletonScope();
            Bind<ITableCollection>().To<TableCollection>().InTransientScope();
            Bind<IExtendedTable>().To<ExtendedTable>().InTransientScope();

            Bind<ITaskServices>().To<TaskServices>().InSingletonScope();

        }
    }


}
