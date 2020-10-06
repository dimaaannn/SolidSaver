using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SolidDrawing;
//using SolidWorks;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;
using System.IO;

namespace SolidApp
{
    class Program
    {
        static void Main(string[] args)
        {

            ISldWorks swApp;
            swApp = SolidTools.GetSWApp();
            ModelDoc2 swModel = swApp.ActiveDoc;

            var swModelCls = new SolidApp.SwModelManager(swModel);

            Console.WriteLine("DocType = " + swModelCls.DocType);
            //Console.WriteLine("Is saved = " + swModelCls.Draw2Pdf()) ;

            Console.WriteLine("GetParam = {0}", swModelCls.PrpMan.GetParam("Наименование")); 
            Console.WriteLine("GetActiveConfig {0}", swModelCls.PrpMan.GetActiveConf);

            string pathToPart = swModelCls.FilePath;

            //SwFileManager.Tests(pathToPart);
            Console.WriteLine("Draw is {0}", SwFileManager.isDrawExcist(pathToPart) ? "Excist" : "Not excist");

            //DrawingDoc openedDraw;
            //var isOpened = SwFileManager.OpenDraw(pathToPart, out openedDraw);
            //ModelDoc2 openedModel = (ModelDoc2)openedDraw;
            //Console.WriteLine("Draw is opened {0}, type = {1}", isOpened, openedModel.GetTitle());

            SwExporter exporter = new SwExporter(SwFileManager.swApp);
            string pathToSave = swModelCls.FolderPath + swModelCls.FileNameWhithoutExt + ".dxf";
            Console.WriteLine("Success = " + exporter.SaveDxf(swModel, pathToSave));


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

    public class SimpleSaver
        {

        }
}
