using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SolidDrawing;
//using SolidWorks;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;

namespace SolidApp
{
    class Program
    {
        static void Main(string[] args)
        {

            ISldWorks swApp;
            swApp = SolidTools.GetSWApp();
            SldWorks.ModelDoc2 swModel = swApp.ActiveDoc;
            Console.WriteLine(swModel.GetType());
            Debug.WriteLine("Test output");

            //Drawing tests
            SwDrawing swDraw = new SwDrawing(swModel);

            foreach (string s in swDraw.GetSheetNames()){
                Console.WriteLine(s);
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
            


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
