using SolidWorks.Interop.sldworks;
using SWAPIlib.ComConn.Proxy;
using System.Collections.Generic;
using System.IO;

namespace SWAPIlib.BaseTypes
{
    public interface ITarget2
    {
        object GetTarget();
    }

    public interface ITarget2<T> : ITarget2
    {
        new T GetTarget();
    }

    public interface IModelWrapper : ITarget2<ModelDoc2>
    {
        string DocTitle { get; }
        AppDocType DocType { get; }
        ISwModelWrapper ConvertToOldWrapper();
        
    }

    public class ModelWrapper : IModelWrapper
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


}
