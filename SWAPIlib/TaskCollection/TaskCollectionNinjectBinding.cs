using SWAPIlib.BaseTypes;

namespace SWAPIlib.TaskCollection
{
    public class TaskCollectionNinjectBinding : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<IExtendedTableFactory>().To<ExtendedTableFactory>().InSingletonScope();
            Bind<ITableCollection>().To<TableCollection>().InTransientScope();
            Bind<SelectedModelProvider>().To<SelectedModelProvider>().InSingletonScope();
            Bind<IPartProvider<IComponentWrapper>>().To<SelectedComponentProvider>().InSingletonScope();

        }
    }


}
