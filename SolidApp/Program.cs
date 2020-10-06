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
using System.Threading;

namespace SolidApp
{
    class Program
    {
        static string Version = "0.2 Alpha";
        static void Main(string[] args)
        {

            ISldWorks swApp;
            AppConsInterface.Greenings(Version);

            swApp = AppConsInterface.GetSwApp();
            ModelDoc2 swModel;
            do
            {
                swModel = AppConsInterface.GetModel(1000);
            } while (!AppConsInterface.CheckType(swModel, swDocumentTypes_e.swDocPART));

            AppConsInterface.BeginSaving(5);

            var modelSaver = new SimpleSaver(swModel);
            AppConsInterface.ExportDXF(swModel);
            


            //var swModelCls = new SolidApp.SwModelManager(swModel);
            //Console.WriteLine("DocType = " + swModelCls.DocType);

            //Console.WriteLine("Is saved = " + swModelCls.Draw2Pdf()) ;

            //Console.WriteLine("GetParam = {0}", swModelCls.PrpMan.GetParam("Наименование")); 
            //Console.WriteLine("GetActiveConfig {0}", swModelCls.PrpMan.GetActiveConf);

            //SwFileManager.Tests(pathToPart);
            //Console.WriteLine("Draw is {0}", SwFileManager.isDrawExcist(pathToPart) ? "Excist" : "Not excist");

            //DrawingDoc openedDraw;
            //var isOpened = SwFileManager.OpenDraw(pathToPart, out openedDraw);
            //ModelDoc2 openedModel = (ModelDoc2)openedDraw;
            //Console.WriteLine("Draw is opened {0}, type = {1}", isOpened, openedModel.GetTitle());

            //SwExporter exporter = new SwExporter(SwFileManager.swApp);
            //string pathToSave = swModelCls.FolderPath + swModelCls.FileNameWhithoutExt + ".dxf";
            //Console.WriteLine("Success = " + exporter.SaveDxf(swModel, pathToSave));


            Console.WriteLine("Press any key");
            Console.ReadKey();

        }
    }

    public class SolidTools
    {
        static string progId = "SldWorks.Application";

        public static ISldWorks GetSWApp()
        {
            var progType = System.Type.GetTypeFromProgID(progId);

            ISldWorks swApp = System.Activator.CreateInstance(progType) as SolidWorks.Interop.sldworks.ISldWorks;
            return swApp;
        }
    }

    /// <summary>
    /// Основной функционал программы сохранения
    /// </summary>
    public class SimpleSaver
    {
        private ModelDoc2 _swModel;
        private SwModelManager _swManager;
        private SwExporter _exporter;

        private readonly string filePath;
        private System.Collections.Generic.Dictionary<string, string> _modelParams;
        private string _configName;
        public string folderToSave;

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

            folderToSave = _swManager.FolderPath + @"\Сохранение";
            SwFileManager.CreateFolder(folderToSave);

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

            }

            _configName = _swManager.PrpMan.GetActiveConf;
            //BROKEN
            Debug.WriteLine("GetDocParams: Config name = {0}\nFailed params count = {1}", _configName, paramSet.Count);
            //BROKEN Todo fix return array
            return paramSet.ToArray();
        }

        /// <summary>
        /// Сохранить все листы чертежа, если он существует
        /// </summary>
        /// <param name="pdfFilePath">Имя файла</param>
        /// <param name="swDrawing">Документ чертежа (опционально)</param>
        /// <returns>Success</returns>
        public bool SavePdf(string pdfFilePath = null, DrawingDoc swDrawing = null )
        {
            bool ret = false;
            bool drawExcist = true;

            if (string.IsNullOrEmpty(pdfFilePath))
            {
                pdfFilePath = _swManager.FolderPath +
                    _swManager.FileNameWhithoutExt +
                    ".pdf";
            }

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

        public bool SaveDxf(string dxfFileName = null)
        {
            bool ret = false;
            if(string.IsNullOrEmpty(dxfFileName))
            {
                dxfFileName = _swManager.FolderPath +
                    _swManager.FileNameWhithoutExt +
                    ".dxf";
            }

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
        public bool SaveModelCopy(string copyFileName = null)
        {
            if (string.IsNullOrEmpty(copyFileName))
            {
                copyFileName = _swManager.FolderPath +
                    _swManager.FileNameWhithoutExt +
                    "_copy"+
                    _swManager.GetFileExtension;
            }
            bool ret = false;
            ret = _exporter.Copy(_swModel, copyFileName, true);
            return ret;
        }

    }

    /// <summary>
    /// Консольный интерфейс
    /// </summary>
    public static class AppConsInterface
    {

        enum ColorMode
        {
            Default,
            Title,
            Warning,
            Info
        }
        private static void SwitchColor(ColorMode colorMode = ColorMode.Default)
        {
            switch (colorMode)
            {
                case ColorMode.Default:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case ColorMode.Title:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case ColorMode.Warning:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ColorMode.Info:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
        }

        /// <summary>
        /// Work with console
        /// </summary>
        public static class ConsStringManager
        {
            public delegate bool WaitAnimation(ref int charLenght);

            /// <summary>
            /// Очистить строку
            /// </summary>
            /// <param name="lineNum">Номер строки</param>
            public static void ClearLine(int lineNum)
            {
                Console.CursorTop = lineNum;
                Console.CursorLeft = 0;
                Console.Write(new string(' ', Console.WindowWidth));
                Console.CursorTop = lineNum;
            }

            /// <summary>
            /// Анимация ожидания
            /// </summary>
            /// <param name="processTime">Время ожидания в мс</param>
            public static void SleepAnimation(int processTime = 700)
            {
                int printTime = 200;
                int repeats = processTime / printTime;

                while (repeats > 0)
                {
                    for(int i = 5; i > 0; --i)
                    {
                        Console.Write(". ");
                        Thread.Sleep(printTime);
                    }
                    ClearLine(Console.CursorTop);
                    repeats--;
                }
            }

        }

        public static void Greenings(string appversion = "")
        {
            string text = $"Welcome to SolidApp {appversion}\n";
            Console.Title = "SolidApp ";

            SwitchColor(ColorMode.Title);
            Console.WriteLine(text);
            SwitchColor(ColorMode.Default);

        }

        /// <summary>
        /// Получить экземпляр программы SolidWorks
        /// </summary>
        /// <param name="sleepTime">Время ожидания</param>
        /// <returns></returns>
        public static ISldWorks GetSwApp(int sleepTime = 1000)
        {
            ISldWorks swApp = null;

            Console.CursorLeft = 0;
            Console.CursorTop = 3;
            Console.WriteLine("Ожидание запуска SolidWorks");
            SwitchColor(ColorMode.Warning);
            Console.WriteLine("Не нужно запускать приложение ДО запуска SolidWorks. \n" +
                "Теперь Иди в диспетчер и убивай процесс");
            SwitchColor(ColorMode.Default);
            while (swApp is null)
            {
                swApp = SolidTools.GetSWApp();
                if (swApp is null)
                    ConsStringManager.SleepAnimation();
            }
            ConsStringManager.ClearLine(3);
            ConsStringManager.ClearLine(4);
            ConsStringManager.ClearLine(5);
            return swApp;
        }

        /// <summary>
        /// Получить экземпляр активного документа
        /// </summary>
        /// <param name="sleepTime">Время ожидания</param>
        /// <returns></returns>
        public static ModelDoc2 GetModel(int sleepTime = 1000)
        {
            var swApp = SwModelManager.swApp;
            ModelDoc2 swModel = null;

            Console.CursorLeft = 0;
            Console.CursorTop = 3;
            Console.WriteLine("Ожидание открытия документа");
            SwitchColor(ColorMode.Info);
            Console.WriteLine("На текущий момент поддерживаются только отдельные детали");
            SwitchColor(ColorMode.Default);

            while (swModel is null)
            {
                swModel = swApp.ActiveDoc;
                if(swModel is null)
                    ConsStringManager.SleepAnimation();
            }
            ConsStringManager.ClearLine(3);
            ConsStringManager.ClearLine(4);
            return swModel;
        }

        /// <summary>
        /// Проверка типа modelDoc2 на соответствие
        /// </summary>
        /// <param name="swModel">Модель</param>
        /// <param name="swDocType">Тип</param>
        /// <returns>Совпадает</returns>
        public static bool CheckType(ModelDoc2 swModel, swDocumentTypes_e swDocType)
        {
            bool ret = false;
            if((swDocumentTypes_e)swModel.GetType() == swDocType)
                ret = true;

            ConsStringManager.ClearLine(5);
            Console.CursorTop = 5;
            if (!ret)
            {
                Console.CursorTop = 6;
                SwitchColor(ColorMode.Warning);
                Console.Write("Некорректный тип открытого документа");
                SwitchColor(ColorMode.Default);
                Console.CursorTop = 4;
                Console.CursorLeft = 0;
                ConsStringManager.SleepAnimation();

            }
            else
            {
                Console.CursorTop = 4;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Документ {0} загружен успешно", swDocType);
                SwitchColor(ColorMode.Default);
                ConsStringManager.ClearLine(5);
                ConsStringManager.ClearLine(6);
                Console.CursorTop = 5;
                Console.CursorLeft = 0;
            }

            return ret;
        }

        public static void BeginSaving(int stringNum)
        {
            Console.CursorTop = stringNum;
            Console.CursorLeft = 0;
            SwitchColor(ColorMode.Info);
            Console.WriteLine("Производится попытка сохранения");
            SwitchColor(ColorMode.Default);
        }

        public static bool ExportDXF(ModelDoc2 swModel, string pathDxf = null)
        {
            bool ret = false;
            var saver = new SimpleSaver(swModel);
            ret = saver.SaveDxf(pathDxf);
            Console.CursorTop = 6;
            Console.CursorLeft = 0;
            Console.WriteLine("DXF is {0}", ret ? "saved" : "not saved");
            return ret;

        }

    }
}
