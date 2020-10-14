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
using System.Security.Policy;

namespace SolidApp
{
    class Program
    {
        static string Version = "0.4 Beta";
        static void Main(string[] args)
        {

            ISldWorks swApp;
            ModelDoc2 swModel;

            string rootFolder;
            string workFolder;
            string name;
            string partName;

            AppConsole.Greenings(Version);
            AppConsole.RunSW();
            AppConsole.LoadSwApp();

            do
            {


                //Загрузить активный документ
                swModel = AppConsole.LoadActiveDoc();
                var part = new SwModelManager(swModel);

                //Получить информацию из открытого документа
                name = part.PrpMan.Title;
                partName = part.PrpMan.GetParam("Обозначение");

                string newFolderName = @"Экспорт/";
                rootFolder = part.FolderPath;
                workFolder = rootFolder + "/" + newFolderName;

                

                //Отобразить информацию на экране
                AppConsole.ShowDocType(part);
                AppConsole.ShowDocName(part);
                AppConsole.ShowProp(part);

                //Экспорт
                if ((int)part.DocType == 1 || (int)part.DocType == 2)
                {

                    Console.WriteLine("\nДля сохранения нажмите пробел. Для отмены, другую кнопку");
                    var userAnswer = Console.ReadKey(true);
                    //Console.WriteLine("char pressed = " + userAnswer.KeyChar);
                    if (userAnswer.Key == ConsoleKey.Spacebar)
                    {
                        // Создать директорию
                        if (!Directory.Exists(workFolder))
                            Directory.CreateDirectory(workFolder);

                        string savingName = partName + " - " + name;
                        AppConsole.SaveDXF(part, workFolder, savingName + ".dxf");

                        AppConsole.SaveCopy(part, workFolder, savingName + part.GetFileExtension);

                        AppConsole.SavePDF(part, workFolder, savingName + ".pdf");

                    }
                }
                else
                {
                    AppConsole.SwitchColor(AppConsole.ColorMode.Warning);
                    Console.WriteLine("\nСохранение невозможно.");
                    AppConsole.SwitchColor();
                }

            }
            while (AppConsole.ReloadModel());

            Console.WriteLine("Пока-пока");
            Thread.Sleep(400);

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
            //_exporter = new SwExporter(SwModelManager.swApp);
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
                //ret = _exporter.SaveDxf(_swModel, dxfFileName);
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

        public enum ColorMode
        {
            Default,
            Title,
            Warning,
            Info
        }
        public static void SwitchColor(ColorMode colorMode = ColorMode.Default)
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
                Console.WriteLine("{0, 23} {1}", "Тип открытой детали: ", StringManager.DocTypeName(docType));
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
                Console.WriteLine("{0, 23} {1}", "Имя документа: ", swModelMan.FileName);
                ret = true;
            }
            return ret;
        }
        /// <summary>
        /// Написать список параметров сборки или детали
        /// </summary>
        /// <param name="swModelMan"></param>
        public static void ShowProp(SwModelManager swModelMan)
        {
            bool ret = false;
            const int offset = 23;
            int lineNum = 6;
            
            Console.CursorTop = lineNum;
            swDocumentTypes_e doctype = swModelMan.DocType;
            if (doctype == swDocumentTypes_e.swDocPART || doctype == swDocumentTypes_e.swDocASSEMBLY)
            {

                string NameProp = swModelMan.PrpMan.Title;
                string parnNum = swModelMan.PrpMan.GetParam("Обозначение");
                string configName = swModelMan.PrpMan.GetActiveConf;
                bool isDrawExist = SwFileManager.isDrawExcist(swModelMan.FilePath);
                bool isSheet = swModelMan.PrpMan.isSheet;

                string DrawIsFound = isDrawExist ? "Найден" : "Не найден";
                string isSheetMetal = isSheet ? $"Листовая - {swModelMan.PrpMan.GetSheetThickness}мм" : "Не листовая";

                //Clear console below
                for (int i = lineNum; i < lineNum + 5; ++i)
                {
                    StringManager.ClearLine(i);
                }

                Console.CursorLeft = 0;
                Console.CursorTop = lineNum;

                Console.WriteLine($"{"Обозначение: ",offset} {parnNum}");
                Console.WriteLine($"{"Наименование: ",offset} {NameProp}");
                Console.WriteLine($"{"Активная конфигурация: ",offset} {configName}");
                Console.WriteLine($"{"Одноимённый чертёж: ",offset} {DrawIsFound}");
                Console.WriteLine($"{"Тип детали: ",offset} {isSheetMetal}");
            }
            else
            {
                Console.WriteLine($"{"Имя чертежа: ",offset} {swModelMan.swModel.GetTitle()}");
            }
        }


        public static bool ReloadModel()
        {
            bool ret;
            ConsoleKeyInfo userAnswer;
            int lineNum = 15;
            Console.CursorTop = lineNum;
            StringManager.ClearLine(lineNum);
            Console.CursorLeft = 0;

            Console.WriteLine("Для выхода нажмите Esc, перезагрузка модели - пробел");
            userAnswer = Console.ReadKey(true);

            switch (userAnswer.Key)
            {
                case ConsoleKey.Spacebar:
                    ret = true;
                    break;
                case ConsoleKey.Escape:
                    ret = false;
                    break;
                default:
                    ret = true;
                    break;
            }

            for(int i = lineNum; i >= 1; --i)
            {
                StringManager.ClearLine(i);
            }

            return ret;
        }

        /// <summary>
        /// Сохранить развёртку
        /// </summary>
        /// <param name="swModelMan"></param>
        /// <param name="folder">Папка сохранения</param>
        /// <param name="name">Имя файла с расширением</param>
        /// <returns></returns>
        public static bool SaveDXF(SwModelManager swModelMan, string folder = null, string name = null)
        {
            bool ret = false;
            int lineNum = 12;
            const int offset = 23;
            Console.CursorLeft = 0;
            //Console.CursorTop = lineNum;

            if (string.IsNullOrEmpty(folder))
                folder = swModelMan.FolderPath;

            if (string.IsNullOrEmpty(name))
                name = swModelMan.FileNameWhithoutExt + ".dxf";


            StringManager.ClearLine(lineNum);
            Console.Write($"{"Сохранение DXF: ",offset}");

            if (swModelMan.PrpMan.isSheet)
            {
                ret = swModelMan.Export.SaveDxf(folder + name);
            }

            Console.Write(ret ? " OK\n" : " Не сохранено\n");
            return ret;
        }
        /// <summary>
        /// Сохранить копию документа
        /// </summary>
        /// <param name="swModelMan"></param>
        /// <param name="folder">Папка сохранения</param>
        /// <param name="name">Имя файла с расширением</param>
        /// <returns></returns>
        public static bool SaveCopy(SwModelManager swModelMan, string folder = null, string name = null)
        {
            bool ret = false;
            int lineNum = 13;
            const int offset = 23;
            Console.CursorLeft = 0;

            if (string.IsNullOrEmpty(folder))
                folder = swModelMan.FolderPath;

            if (string.IsNullOrEmpty(name))
                name = swModelMan.FileName;


            StringManager.ClearLine(lineNum);
            Console.Write($"{"Сохранение копии: ",offset}");

            ret = swModelMan.SaveAsCopy(folder + name);

            Console.Write(ret ? " OK\n" : " Не сохранено\n");
            return ret;
        }

        /// <summary>
        /// Сохранить PDF из чертежа с тем же именем
        /// </summary>
        /// <param name="swModelMan"></param>
        /// <param name="folder">Папка сохранения</param>
        /// <param name="name">Имя файла с расширением</param>
        /// <returns></returns>
        public static bool SavePDF(SwModelManager swModelMan, string folder = null, string name = null)
        {
            bool ret = false;
            int lineNum = 14;
            const int offset = 23;
            Console.CursorLeft = 0;

            if (string.IsNullOrEmpty(folder))
                folder = swModelMan.FolderPath;

            if (string.IsNullOrEmpty(name))
                name = swModelMan.FileNameWhithoutExt + ".pdf";


            StringManager.ClearLine(lineNum);
            Console.Write($"{"Сохранение PDF: ",offset}");

            DrawingDoc DrawDoc;

            string drawName = Path.ChangeExtension(swModelMan.FileName, "SLDDRW");
            var openedDraw = SwProcess.swApp.GetOpenDocument(drawName);
            bool docIsOpened = false;
            if (!(openedDraw is null))
                docIsOpened = true;

            if (SwFileManager.OpenDraw(swModelMan.FilePath, out DrawDoc))
            {
                var model = (ModelDoc2)DrawDoc;
                
                ret = swModelMan.Export.SavePdf(DrawDoc, folder + name, true);
                
                if(!docIsOpened)
                    SwProcess.swApp.CloseDoc(model.GetTitle());
            }

            Console.Write(ret ? " OK\n" : " Не сохранено\n");
            return ret;
        }



    }
}
