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
            var swExp = new SwExporter(swApp);

            Console.WriteLine("DocType = " + swModelCls.DocType);
            //Console.WriteLine("Is saved = " + swModelCls.Draw2Pdf()) ;

            string pathToFile =  @"\\sergeant\Техотдел\Технологический - Общие документы\Общая\Красиков\VBA\SolidWorks\Тестовая сборка\Test.pdf";


            //Console.WriteLine("GetSheetThickness = {0}", swModelCls.PrpMan.GetSheetThickness); //Проверка isSheet

            Console.WriteLine("GetParam = {0}", swModelCls.PrpMan.GetParam("Наименование")); 

            Console.WriteLine("GetActiveConfig {0}", swModelCls.PrpMan.GetActiveConf);
            
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

    public static class TestFunctions
    {
        public static void GetFileAttr(string path)
        {
            FileAttributes fAttr = File.GetAttributes(path);
            FileAttributes fAttr2 = FileAttributes.Archive;

            Console.WriteLine(fAttr == fAttr2);

        }
    }
}
