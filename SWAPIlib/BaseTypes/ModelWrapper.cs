﻿using SolidWorks.Interop.sldworks;
using SWAPIlib.ComConn.Proxy;
using System.IO;

namespace SWAPIlib.BaseTypes
{
    public interface ITarget
    {
        object GetTarget();
    }

    public interface ITarget<T> : ITarget
    {
        new T GetTarget();
    }

    public interface IModelWrapper : ITarget<ModelDoc2>
    {
        string DocTitle { get; }
        AppDocType DocType { get; }
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
        ModelDoc2 ITarget<ModelDoc2>.GetTarget() => swModel;

    }


}
