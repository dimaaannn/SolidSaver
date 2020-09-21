using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using SolidWorks;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;

namespace SolidApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //var progId = "SldWorks.Application";

            //var progType = System.Type.GetTypeFromProgID(progId);

            //var swApp = System.Activator.CreateInstance(progType) as SolidWorks.Interop.sldworks.ISldWorks;
            //swApp.Visible = true;

            ISldWorks swApp;
            swApp = SolidTools.GetSWApp();
            SldWorks.ModelDoc2 swModel = swApp.ActiveDoc;
            Console.WriteLine(swModel.GetType());
            Debug.WriteLine("Test output");
            


        }
    }

    class SolidTools
    {
        public static ISldWorks GetSWApp()
        {
            var progId = "SldWorks.Application";

            var progType = System.Type.GetTypeFromProgID(progId);

            ISldWorks swApp = System.Activator.CreateInstance(progType) as SolidWorks.Interop.sldworks.ISldWorks;
            return swApp;
        }
    }
}
