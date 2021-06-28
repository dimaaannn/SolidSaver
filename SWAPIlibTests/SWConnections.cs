using SolidWorks.Interop.sldworks;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlibTests
{
    public static class SWConnections
    {
        public static ModelDoc2 GetActiveModel()
        {
            SwAppControl.Connect();
            var appModel = ModelClassFactory.ActiveDoc;
            return appModel.SwModel;
        }

        public static ITargetTable GetActiveModelTarget()
        {
            return new TargetTable(GetActiveModel()) { Name = "SomeName" };
        }
    }
}
