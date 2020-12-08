using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{
    public class MainModel// : VisualInterface.IMainModel
    {
        public AppModel RootModel { get; set; }
        public AppDocType DocType 
        { get
            {
                if (RootModel is null)
                    return AppDocType.swNONE;
                else
                {
                    return RootModel.DocType;
                }
            }
        }
        public string Title => RootModel?.Title; //TODO add override for types
        public string Path => RootModel?.Path;
        public IList<ISwProperty> PropList { get => RootModel?.PropList; }
        public IFileModelProp GlobalModelProp { get => RootModel?.GlobalModelProp; }

        public List<IAppComponent> SubComponents { get => _subComponents; }
        private List<IAppComponent> _subComponents;

        public ISwProperty MainInfoProp { get; set; }

        public bool GetMainModel(string pathToModel = null)
        {
            bool ret = false;
            if (String.IsNullOrEmpty(pathToModel))
            {
                RootModel = ModelFactory.GetModel(SwAppControl.MainModel);
            }
            return ret;
        }
        public bool GetSubComponents(bool TopLevelOnly)
        {
            bool ret = false;
            if(RootModel is ISwAssembly swAssembly)
            {
                _subComponents = swAssembly.GetComponents(TopLevelOnly);
                if (_subComponents.Count > 0)
                    ret = true;
            }
            return ret;
        }

        public event EventHandler<SwEventArgs> CloseRootModel;

    }


}
