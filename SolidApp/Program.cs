using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SolidDrawing;
//using SolidWorks;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;
using System.IO;
using SwConst;
using System.Linq;

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
        private ModelDoc2 _swModel;
        private SwModelManager _swManager;
        private SwExporter _exporter;

        private readonly string filePath;
        private System.Collections.Generic.Dictionary<string, string> _modelParams;
        private string _configName;

        /// <summary>
        /// Создать класс
        /// </summary>
        /// <param name="swModel">Модель ТОЛЬКО DocPart</param>
        public SimpleSaver(ModelDoc2 swModel)
        {
            //Check model and save as param
            Debug.WriteLine("SimpleSaver: Begin checking file");
            if (CheckModel(swModel))
            {
                Debug.WriteLine("Class simpleSaver created");
                _swModel = swModel;
            }

            _modelParams = new Dictionary<string, string>();
            _exporter = new SwExporter(SwModelManager.swApp);
            _swManager = new SwModelManager(_swModel);

            GetDocParams();

        }
        /// <summary>
        /// Проверить модель перед созданием класса
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        private static bool CheckModel(ModelDoc2 swModel)
        {
            bool ret = false;
            if (!(swModel is null))
            {
                if (swModel.GetType() == (int)swDocumentTypes_e.swDocPART)
                    ret = true;
                else
                    throw new ArgumentException("SimpleSaver: Document is not PartDoc");
            }
            else
                throw new NullReferenceException("SimpleSaver - modelReference is not exist");
            Debug.WriteLine("CheckModel: Succes");
            return ret;
        }

        //public static ModelDoc2 GetDocument() { }

        /// <summary>
        /// Получить параметры из детали
        /// </summary>
        /// <returns>Список НЕ полученных параметров</returns>
        private string[] GetDocParams()
        {
            Debug.WriteLine("GetDocParams: Begin");
            var paramSet = new HashSet<string>() {
                "Обозначение",
                "Наименование",
                "Material"
            };

            foreach (string paramname in paramSet)
            {
                _modelParams.Add(paramname, _swManager.PrpMan.GetParam(paramname));
                if (string.IsNullOrEmpty(_modelParams[paramname]))
                    _modelParams[paramname] = "";
                else
                    paramSet.Remove(paramname);
            }

            _configName = _swManager.PrpMan.GetActiveConf;

            Debug.WriteLine("GetDocParams: Config name = {0}\nFailed params count = {1}", _configName, paramSet.Count);
            return paramSet.ToArray();
        }

        /// <summary>
        /// Сохранить все листы чертежа, если он существует
        /// </summary>
        /// <param name="pdfFilePath">Имя файла</param>
        /// <param name="swDrawing">Документ чертежа (опционально)</param>
        /// <returns>Success</returns>
        public bool SavePdf(string pdfFilePath, DrawingDoc swDrawing = null )
        {
            bool ret = false;
            bool drawExcist = true;
            Debug.WriteLine("SavePdf: Path = " + pdfFilePath);
            if (swDrawing is null)
                Debug.WriteLine("SavePdf: Try opening draw doc");
                drawExcist = SwFileManager.OpenDraw(_swManager.FilePath, out swDrawing);

            if (drawExcist)
                Debug.WriteLine("SavePdf: Try save draw to pdf");
            ret = _exporter.SavePdf((ModelDoc2)swDrawing, pdfFilePath, true);

            Debug.WriteLine("SavePdf: Saving {0}", ret ? "Success" : "Fail");
            return ret;
        }

        public bool SaveDxf(string dxfFileName)
        {
            bool ret = false;
            if (_swManager.PrpMan.isSheet)
            {
                ret = _exporter.SaveDxf(_swModel, dxfFileName);
            }
            else
                throw new FormatException("SaveDxf: Экспорт в DXF невозможен, деталь не является листовой");
            return ret;
        }

        /// <summary>
        /// Сохранить копию модели средствами SolidWorks
        /// </summary>
        /// <param name="copyFileName"></param>
        /// <returns></returns>
        public bool SaveModelCopy(string copyFileName)
        {
            bool ret = false;
            ret = _exporter.Copy(_swModel, copyFileName, true);
            return ret;
        }




    }
}
