using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{
    public class MainModel : VisualInterface.IMainModel 
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
        public string Title => RootModel.Title; //TODO add override for types
        public string Path => RootModel.Path;
        public List<ISwProperty> PropList { get; }
        public IFileModelProp GlobalModelProp { get; }

        public List<ISwModel> SubComponents { get; private set; }

        public bool GetRootModel(string pathToModel = null)
        {
            bool ret = false;
            if (String.IsNullOrEmpty(pathToModel))
            {
                RootModel = new AppModel(SwAppControl.MainModel);
            }
            return ret;
        }
        public bool GetSubComponents()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<SwEventArgs> CloseRootModel;

    }

    public static class PropertySetFactory
    {
        public static List<ISwModel> GetDefault(AppModel appModel)
        {
            throw new NotImplementedException();
        }

        public static List<ISwModel> Assembly(AppModel appModel)
        {
            var retList = new List<ISwModel>();
            return retList;
        }



    }
}
