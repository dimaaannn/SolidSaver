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
            Console.WriteLine(swModel.GetType());
            Debug.WriteLine("Test output");

            //Drawing tests
            //SwDrawing swDraw = new SwDrawing(swModel);

            //foreach (string s in swDraw.SheetNames){
            //    Console.WriteLine(s);
            //}
            //try
            //{
            //    Console.WriteLine(swDraw.SavePdf() ? "PDF is saved" : "PDF is NOT saved");
            //}
            //catch(FileLoadException e)
            //{
            //    Console.WriteLine("File is locked " + e.Message);
            //}

            var swModelCls = new SolidApp.SwModelManager(swModel);
            var swExp = new SwExporter(swApp);

            string savePath = @"\\sergeant\Техотдел\Технологический - Общие документы\Общая\Красиков\VBA\SolidWorks\Тестовая сборка\2670 Основа топпера-Copy.SLDPRT";

            Console.WriteLine("DocType = " + swModelCls.DocType);
            Console.WriteLine("Is saved = " + swExp.Copy(swModel, savePath, false));

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
