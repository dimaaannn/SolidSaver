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
            ModelDoc2 swModel;

            AppConsole.Greenings(Version);
            AppConsole.RunSW();
            AppConsole.LoadSwApp();


            swApp = SwProcess.swApp;
            swModel = AppConsole.LoadActiveDoc();

            
            var swProc = SwProcess.swProcess;


            Console.WriteLine("SWapp is exited " + SwProcess.IsRunning);


            //swApp = AppConsInterface.GetSwApp();
            //ModelDoc2 swModel;
            //do
            //{
            //    swModel = AppConsInterface.GetModel(1000);
            //} while (!AppConsInterface.CheckType(swModel, swDocumentTypes_e.swDocPART));

            //AppConsInterface.BeginSaving(5);

            //var modelSaver = new SimpleSaver(swModel);
            //AppConsInterface.ExportDXF(swModel);
            



            Console.WriteLine("Press any key");
            Console.ReadKey();

        }
    }


    //TODO убрать нафиг
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
    public static class AppConsole
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
        public static class StringManager
        {

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
            /// <param name="stringNum">Номер строки</param>
            /// <param name="processTime">Время ожидания в мс</param>
            public static void SleepAnimation(int stringNum = -1, int processTime = 700)
            {
                int printTime = 100;
                int StringLenght = 10;
                int repeats = processTime / printTime / StringLenght;

                if(stringNum != -1)
                {
                    Console.CursorTop = stringNum;
                }

                Console.CursorVisible = false; //Выключить курсор
                try
                {

                    while (repeats > 0)
                    {
                        for (int i = StringLenght; i > 0; --i)
                        {
                            Thread.Sleep(printTime);
                            Console.Write("- ");
                        }
                        Thread.Sleep(printTime);
                        ClearLine(Console.CursorTop);
                        //repeats--;

                        Thread.Sleep(printTime);
                        ClearLine(Console.CursorTop);

                    }
                }
                catch (ThreadInterruptedException e) { }
                finally
                {
                    ClearLine(Console.CursorTop);
                    Console.CursorVisible = true; //Включить курсор
                }

            }

            /// <summary>
            /// Название типа детали
            /// </summary>
            /// <param name="docType"></param>
            /// <returns></returns>
            public static string DocTypeName(swDocumentTypes_e docType)
            {
                string typeMessage;
                switch (docType)
                {
                    case swDocumentTypes_e.swDocASSEMBLY:
                        typeMessage = "Сборка";
                        break;
                    case swDocumentTypes_e.swDocDRAWING:
                        typeMessage = "Чертёж";
                        break;
                    case swDocumentTypes_e.swDocPART:
                        typeMessage = "Деталь";
                        break;
                    default:
                        typeMessage = "Что то непонятное";
                        break;
                }
                return typeMessage;
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
        /// Приглашение запустить SW
        /// </summary>
        /// <param name="sleepTime">Время ожидания</param>
        /// <returns></returns>
        public static bool RunSW(int sleepTime = 1000)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 3;
            Console.WriteLine("Ожидание запуска SolidWorks");

            Thread Animation = new Thread(() => StringManager.SleepAnimation(2, 5000));
            Animation.Start(); // Запуск потока с анимацией
            while (SwProcess.IsRunning == false)
            {
                Thread.Sleep(500);
                //StringManager.SleepAnimation(stringNum: 2, processTime: sleepTime);
            }

            StringManager.ClearLine(3);
            Animation.Interrupt(); //Прервать поток анимации загрузки
            return true;
        }

        /// <summary>
        /// Сообщение о загрузке SW
        /// </summary>
        /// <returns></returns>
        public static bool LoadSwApp()
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 3;
            Console.WriteLine("Ожидание загрузки SolidWorks");

            Thread Animation = new Thread( () => StringManager.SleepAnimation(2, 5000));
            Animation.Start(); // Запуск потока с анимацией

            while(SwProcess.swApp is null)
            {
                Thread.Sleep(500);
                //StringManager.SleepAnimation(2);
            }
            StringManager.ClearLine(3);
            Animation.Interrupt(); //Прервать поток анимации загрузки
            return true;
        }        

        /// <summary>
        /// Ожидание активного документа
        /// </summary>
        /// <returns></returns>
        public static ModelDoc2 LoadActiveDoc()
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 3;
            Console.WriteLine("Откройте деталь");

            Thread Animation = new Thread( () => StringManager.SleepAnimation(2, 5000));
            Animation.Start(); // Запуск потока с анимацией

            ModelDoc2 swModel;
            do
            {
                
                swModel = SwProcess.swApp.ActiveDoc;
                Thread.Sleep(500);
                //StringManager.SleepAnimation(2);
            }
            while (swModel is null);

            StringManager.ClearLine(3);
            Animation.Interrupt(); //Прервать поток анимации загрузки
            StringManager.ClearLine(2);
            return swModel;
        }

        /// <summary>
        /// Напечатать тип детали
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static bool ShowDocType(SwModelManager swModelMan)
        {
            swDocumentTypes_e docType;
            bool ret = false;
            int lineNum = 3;
            Console.CursorLeft = 0;
            Console.CursorTop = lineNum;
            Console.WriteLine("Открытый документ отсутствует");
            if(!(swModelMan is null))
            {
                docType = swModelMan.DocType;

                StringManager.ClearLine(lineNum);
                Console.Write("Тип открытой детали: " + StringManager.DocTypeName(docType));
                ret = true;
            }

            return ret;
        }
        /// <summary>
        /// Напечатать имя файла
        /// </summary>
        /// <param name="swModelMan"></param>
        /// <returns></returns>
        public static bool ShowDocName(SwModelManager swModelMan)
        {
            bool ret = false;
            int lineNum = 4;
            Console.CursorLeft = 0;
            Console.CursorTop = lineNum;
            Console.WriteLine("Отсутствует");
            if (!(swModelMan is null))
            {
                StringManager.ClearLine(lineNum);
                Console.Write("Имя документа :" + swModelMan.FileName);
                ret = true;
            }
            return ret;
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

            StringManager.ClearLine(5);
            Console.CursorTop = 5;
            if (!ret)
            {
                Console.CursorTop = 6;
                SwitchColor(ColorMode.Warning);
                Console.Write("Некорректный тип открытого документа");
                SwitchColor(ColorMode.Default);
                Console.CursorTop = 4;
                Console.CursorLeft = 0;
                StringManager.SleepAnimation();

            }
            else
            {
                Console.CursorTop = 4;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Документ {0} загружен успешно", swDocType);
                SwitchColor(ColorMode.Default);
                StringManager.ClearLine(5);
                StringManager.ClearLine(6);
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
