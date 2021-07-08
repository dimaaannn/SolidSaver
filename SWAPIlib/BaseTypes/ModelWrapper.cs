using SolidWorks.Interop.sldworks;
using SWAPIlib.ComConn.Proxy;
using System.Collections.Generic;
using System.IO;

namespace SWAPIlib.BaseTypes
{

    public class WrapperNinjectBinding : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<IPartWrapperFactory>().To<PartWrapperFactory>().InSingletonScope();
        }
    }

    public interface ITarget2
    {
        object GetTarget();
    }

    public interface ITarget2<T> : ITarget2
    {
        new T GetTarget();
    }

    public interface IPartWrapper : ITarget2
    {
        string Name { get; }
        AppDocType DocType { get; }
    }
    public interface IModelWrapper : ITarget2<ModelDoc2>
    {
        string DocTitle { get; }
        AppDocType DocType { get; }
        ISwModelWrapper ConvertToOldWrapper();
        
    }

    public class ModelWrapper : IModelWrapper, IPartWrapper
    {
        private readonly ModelDoc2 swModel;
        private readonly string title;
        private readonly AppDocType docType;

        public ModelWrapper(ModelDoc2 model)
        {
            this.swModel = model;
            title = GetTitle(swModel);
            docType = PartTypeChecker.GetSWType(swModel);
        }

        string IPartWrapper.Name => DocTitle;
        public string DocTitle => title;
        public AppDocType DocType => docType;

        private string GetTitle(ModelDoc2 model) => ModelProxy.GetName(model);

        public object GetTarget() => swModel;
        ModelDoc2 ITarget2<ModelDoc2>.GetTarget() => swModel;

        public ISwModelWrapper ConvertToOldWrapper()
        {
            return ((SwModelWrapper)this) as ISwModelWrapper;
        }

        public override bool Equals(object obj)
        {
            return obj is ModelWrapper wrapper &&
                   EqualityComparer<ModelDoc2>.Default.Equals(swModel, wrapper.swModel) &&
                   title == wrapper.title;
        }

        public override int GetHashCode()
        {
            int hashCode = 468417109;
            hashCode = hashCode * -1521134295 + EqualityComparer<ModelDoc2>.Default.GetHashCode(swModel);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(title);
            return hashCode;
        }

        public static implicit operator SwModelWrapper(ModelWrapper modelwrapper)
        {
            return new SwModelWrapper(modelwrapper.swModel);
        }


    }


    public interface IComponentWrapper : IPartWrapper, ITarget2<Component2>
    {
        IModelWrapper GetModel();
    }

    public class ComponentWrapper : ITarget2<Component2>, IComponentWrapper
    {
        private readonly IPartWrapperFactory partWrapperFactory;
        private readonly Component2 component2;
        private readonly string name;
        private readonly AppDocType docType;

        private IModelWrapper ModelWrapper;

        public ComponentWrapper(Component2 component, IPartWrapperFactory partWrapperFactory)
        {
            component2 = component;
            name = SWAPIlib.ComConn.Proxy.ComponentProxy.GetName(component2);
            docType = AppDocType.swCOMPONENT;
            this.partWrapperFactory = partWrapperFactory;
        }

        public string Name => name;
        public AppDocType DocType => docType;

        public IModelWrapper GetModel()
        {
            if(ModelWrapper == null)
            {
                ModelDoc2 model = SWAPIlib.ComConn.Proxy.ComponentProxy.GetModelDoc2(component2);
                ModelWrapper = partWrapperFactory.GetModelWrapper(model);
            }
            return ModelWrapper;
        }

        public Component2 GetTarget() => component2;
        object ITarget2.GetTarget() => component2;
    }

    public interface IPartWrapperFactory
    {
        IModelWrapper GetModelWrapper(ModelDoc2 swModel);
        IComponentWrapper GetComponentWrapper(Component2 swComponent);
    }

    public class PartWrapperFactory : IPartWrapperFactory
    {
        public IComponentWrapper GetComponentWrapper(Component2 swComponent)
        {
            return new ComponentWrapper(component: swComponent, this);
        }

        public IModelWrapper GetModelWrapper(ModelDoc2 swModel)
        {
            return new ModelWrapper(swModel);
        }
    }
}
