namespace SWAPIlib.BaseTypes
{
    public class WrapperNinjectBinding : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<IPartWrapperFactory>().To<PartWrapperFactory>().InSingletonScope();
        }
    }
}
