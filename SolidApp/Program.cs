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
        static string Version = "0.43 Beta";

        [STAThread]
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
                name = part.PrpMan.GetParam("Наименование");
                partName = part.PrpMan.GetParam("Обозначение");

                workFolder = WorkFolder.FolderPath ?? part.FolderPath;
                
                
                //Отобразить информацию на экране
                AppConsole.ShowDocType(part);
                AppConsole.ShowDocName(part);
                AppConsole.ShowProp(part);

                //Экспорт
                if ((int)part.DocType == 1 || (int)part.DocType == 2)
                {
                    Console.CursorTop = 18;
                    AppConsole.SwitchColor(AppConsole.ColorMode.Info);
                    Console.WriteLine($"Рабочая папка\n{workFolder}");
                    AppConsole.SwitchColor(AppConsole.ColorMode.Default);
                    Console.CursorTop = 11;
                    Console.WriteLine("Чтобы изменить рабочую папку - Ввод. Для продолжения кнопку пробел");
                    var userAnswer = Console.ReadKey(true);
                    
                    if (userAnswer.Key == ConsoleKey.Spacebar && part.DocType == swDocumentTypes_e.swDocPART)
                    {
                        // Создать директорию
                        if (!Directory.Exists(workFolder))
                            Directory.CreateDirectory(workFolder);

                        if (partName.Length > 3 && name.Length > 3)
                        {
                            string savingName = partName + " - " + name;
                            AppConsole.SaveDXF(part, workFolder, savingName + ".dxf");

                            AppConsole.SaveCopy(part, workFolder, savingName + part.GetFileExtension);

                            AppConsole.SavePDF(part, workFolder, savingName + ".pdf");
                        }
                        else
                        {
                            AppConsole.SwitchColor(AppConsole.ColorMode.Warning);
                            Console.WriteLine("Отсутствует обозначение или описание");
                            AppConsole.SwitchColor(AppConsole.ColorMode.Default);

                        }
                    }

                    //Изменить рабочую папку
                    else if(userAnswer.Key == ConsoleKey.Enter)
                    {
                        WorkFolder.FolderPath = WorkFolder.GetFolderDialog(workFolder);
                        for (int i = 18; i < 25; i++)
                            AppConsole.StringManager.ClearLine(i);
                    }
                    else
                    {
                        AppConsole.SwitchColor(AppConsole.ColorMode.Warning);
                        Console.WriteLine("Сохранение из сборки не поддерживается");
                        AppConsole.SwitchColor();
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
                
        public static void SelectFolder(string defPath = null)
        {
            string text = $"Выберите рабочую папку";

            Console.CursorLeft = 0;
            Console.CursorTop = 2;
            SwitchColor(ColorMode.Info);
            Console.WriteLine(text);
            SwitchColor(ColorMode.Default);
            Thread.Sleep(500);

            WorkFolder.FolderPath = WorkFolder.GetFolderDialog(defPath);

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

        public static bool CheckParams(params string[] args)
        {
            bool result = true;

            if(args.Contains(null) || args.Count() == 0)
            {
                result = false;
            }

            return result;
        }

    }
}
