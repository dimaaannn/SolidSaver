using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SwConst;
using System.Diagnostics;
using SWAPIlib.ComConn.Proxy;

namespace SWAPIlib.BaseTypes
{
    public class AppAssembly : AppModel, IAppAssembly
    {
        public AppAssembly(ModelDoc2 swModel) : base(swModel)
        {
            _swAsm = swModel as AssemblyDoc;
            PropList = new List<ISwProperty>();
            PropList.AddRange(PropertyFactory.ModelProperty.DefaultModelProp(this));
        }
        private AssemblyDoc _swAsm;
        public string ConfigName
        {
            get => ModelConfigProxy.GetActiveConfName(SwModel);
            set => ModelConfigProxy.SetActiveConf(SwModel, value);
        }



        public int ComponentCount(bool TopLevelOnly = true)
        {
            return AsmDocProxy.GetComponentCount(SwModel, TopLevelOnly);
        }

        public List<IAppComponent> GetComponents(bool TopLevelOnly)
        {
            var ret = new List<IAppComponent>();
            var swComponents = AsmDocProxy.GetComponents(SwModel, TopLevelOnly);
            foreach (Component2 comp in swComponents)
            {
                ret.Add(new AppComponent(comp));
            }
            return ret;
        }
    }
}
