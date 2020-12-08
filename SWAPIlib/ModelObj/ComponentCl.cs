using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SWAPIlib
{
    public class AppComponent : ISwModel, IAppComponent
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
        public List<ISwProperty> PropList { get; private set; }

        public string RefConfigName
        {
            get => SwCompModel.ReferencedConfiguration;
            set => SwCompModel.ReferencedConfiguration = value;
        }
        public List<string> ConfigList { get => PartModel?.ConfigList;}
        public bool VisibState
        {
            get => (int)ComponentProxy.GetVisibleStatus(SwCompModel) == 1;
            set => ComponentProxy.SetVisibleStatus(SwCompModel, 
                value ? AppCompVisibility.Visible : AppCompVisibility.Hidden);
        }
        public bool ExcludeFromBOM
        {
            get => SwCompModel.ExcludeFromBOM;
            set => SwCompModel.ExcludeFromBOM = value;
        }
        public event EventHandler<SwEventArgs> CloseFile
        {
            add => throw new NotImplementedException();
            remove { }
        }

        public List<IAppComponent> GetComponents(bool TopLeverOnly)
        {
            var ret = new List<IAppComponent>();
            var components = ComponentProxy.GetChildren(SwCompModel);
            Debug.WriteLine($"GetComponents from {this.FileName} begin");
            foreach (Component2 comp in components)
            {
                ret.Add(new AppComponent(comp));
            }
            Debug.WriteLine(ret.Count > 0 ? "Success" : "No components");

            return ret;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="swComp2"></param>
        public AppComponent(Component2 swComp2)
        {
            if(swComp2 != null)
            {
                PropList = new List<ISwProperty>();
                PropList.AddRange(PropertyObj.CompPropertyFactory.
                    ComponentMainProp(this));

                _swCompModel = swComp2;
                DocType = AppDocType.swNONE;

                var swModel = ComponentProxy.GetModelDoc2(swComp2);

                //Если компонент не погашен
                if (swModel != null)
                {
                    _appModel = ModelFactory.GetModel(swModel);
                    IsExist = true;
                    DocType = _appModel.DocType;
                    PropList.AddRange(_appModel.PropList);
                }
            }
        }

        public IAppComponent GetRootComponent()
        {
            return new AppComponent(ComponentProxy.GetRoot(SwCompModel));
        }

        public IAppComponent GetParent()
        {
            return new AppComponent(ComponentProxy.GetParent(SwCompModel));
        }

        public int GetChildrenCount() => SwCompModel.IGetChildrenCount();

    }
}
