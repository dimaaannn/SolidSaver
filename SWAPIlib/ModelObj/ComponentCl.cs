using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{
    class AppComponent : IAppComponent
    {
        public virtual bool IsExist { get; private set; }
        public AppModel PartModel => _appModel;
        private AppModel _appModel = null;

        public ModelDoc2 SwModel => PartModel?.SwModel;
        public virtual AppDocType DocType { get; private set; }

        public Component2 SwCompModel => _swCompModel;
        private Component2 _swCompModel;

        public AppSuppressionState SuppressionState =>
            PartTypeChecker.GetAppSuppressionState(SwCompModel);

        public string Path => ComponentProxy.GetPathName(SwCompModel);
        public virtual string FileName => System.IO.Path.GetFileName(Path);
        public string Title => ComponentProxy.GetName(_swCompModel);

        public IFileModelProp GlobalModelProp { get; set; }
        public List<ISwProperty> PropList { get; set; }

        public string RefConfigName
        {
            get => SwCompModel.ReferencedConfiguration;
            set => SwCompModel.ReferencedConfiguration = value;
        }
        public AppCompVisibility VisibState
        {
            get => ComponentProxy.GetVisibleStatus(SwCompModel);
            set => ComponentProxy.SetVisibleStatus(SwCompModel, value);
        }
        public event EventHandler<SwEventArgs> CloseFile
        {
            add => throw new NotImplementedException();
            remove { }
        }

        public AppComponent(Component2 swComp2)
        {
            if(swComp2 != null)
            {
                _swCompModel = swComp2;
                DocType = AppDocType.swNONE;

                var swModel = ComponentProxy.GetModelDoc2(swComp2);
                //_swCompModel.GetVisibility
                
                if (swModel != null)
                {
                    _appModel = ModelFactory.GetModel(swModel);
                    DocType = _appModel.DocType;
                }


            }
        }
    }
}
