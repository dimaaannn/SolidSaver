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
using SWAPIlib;
using System.Collections;
using SWAPIlib.ComConn;
using SWAPIlib.BaseTypes;
using SWAPIlib.Controller;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.MProperty;
using SWAPIlib.MProperty.Getters;
using SWAPIlib.Global;

namespace SolidApp
{
    class Program
    {
        static string Version = "0.51 Beta";

        static void Main(string[] args)
        {
            //var swModel = AppConsole.LoadActiveDoc();
            SwAppControl.Connect();

            //Get Raw document
            var appmodel = ModelClassFactory.ActiveDoc;

            //Print root title
            AppAssembly rootAsm = null;
            ModelDoc2 testRawModel = null;
            List<IAppComponent> compList = null;
            if (appmodel.SwModel is AssemblyDoc swAsm)
            {
                rootAsm = new AppAssembly(appmodel.SwModel);
                Console.WriteLine($"RootDoc: {rootAsm.Title}");
                compList = rootAsm.GetComponents(true);
                testRawModel = compList.First().SwModel;
            }

            var swApp = SwAppControl.swApp;


            var rootModelClass = new SWAPIlib.Global.LinkedModel();
            rootModelClass.GetMainModel();
            rootModelClass.TopLevelOnly = true;
            rootModelClass.GetSubComponents();

            //Test component in assembly
            IAppComponent testcomp = rootModelClass.SubComponents.Skip(0).First();
            IAppComponent testcomp2 = rootModelClass.SubComponents.Skip(1).First();
            IComponentControl testcompcontrol = new SWAPIlib.Controller.ComponentControl(testcomp);

            AppModel compAppModel = testcomp.PartModel;
            ModelDoc2 rawmodel = testcomp.SwModel;
            Component2 rawcomp = testcomp.SwCompModel;

            Console.WriteLine($"testedCompName={testcomp.Title}");
            //compAppModel.GlobalModelProp;

            //var test =  swApp.EnumDocuments2();
            //var modelList = new List<IAppModel>();
            //ModelDoc2 tempDoc;
            //int wtf = 0;
            //for(int i = 0; i < 15; i++)
            //{
            //    test.Next(1, out tempDoc, ref wtf);
            //    var model = ModelClassFactory.GetModel(tempDoc);
            //    if(tempDoc?.Visible == true)
            //    {
            //        var visibility = tempDoc.Visible;
            //        modelList.Add(model);
            //    }
            //}

            var openedDocs = OpenedDocs.GetVisibleDocs();
            Console.WriteLine(string.Join(",", openedDocs.Select(doc => doc.FileName)));
            



            Console.ReadKey();
        }



    }


    public class TreeEnumNode :IEnumerator<TreeEnumNode>, IEnumerable<TreeEnumNode>
    {
        public TreeEnumNode()
        {
            SubNodes = new List<TreeEnumNode>();
        }
        public string Name { get; set; }
        public List<TreeEnumNode> SubNodes { get; set; }


        private int position = -1;
        private IEnumerator<TreeEnumNode> subEnum;
        private TreeEnumNode _current;
        public TreeEnumNode Current
        {
            get
            {
                return _current;
            }
        }
        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            if (position < SubNodes.Count - 1)
            {                
                if (subEnum != null && subEnum.MoveNext())
                {
                    _current = subEnum.Current;
                }
                else
                {
                    position++;
                    _current = SubNodes[position];
                    subEnum = _current.GetEnumerator();
                }
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }

        //Testing

        public IEnumerator<TreeEnumNode> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }


    public class RangeContaiter :IEnumerable
    {
        public RangeContaiter()
        {
            RangeList = System.Linq.Enumerable.Range(1, 4).ToList();
        }
        List<int> RangeList;

        public IEnumerator GetEnumerator()
        {
            return RangeList.GetEnumerator();
        }
    }

    public class ContainerContaiter :IEnumerable
    {
        public ContainerContaiter(int count)
        {
            containers = new List<RangeContaiter>(50);

            for(int i = 0; i < count; i++)
            {
                containers.Add(new RangeContaiter());
            }
        }
        public List<RangeContaiter> containers;

        public IEnumerator GetEnumerator()
        {
            foreach(var item in containers)
            {
                foreach(var subitem in item)
                {
                    yield return subitem;
                }
            }
        }
    }

    //    static void PrevMain(string[] args)
    //    {

    //        ISldWorks swApp;
    //        ModelDoc2 swModel;

    //        string rootFolder;
    //        string workFolder;
    //        string name;
    //        string partName;

    //        AppConsole.Greenings(Version);
    //        AppConsole.RunSW();
    //        AppConsole.LoadSwApp();

    //        do
    //        {


    //            //Загрузить активный документ
    //            swModel = AppConsole.LoadActiveDoc();
    //            var part = new SwModelManager(swModel);

    //            //Получить информацию из открытого документа
    //            name = part.PrpMan.Title;
    //            partName = part.PrpMan.GetParam("Обозначение");

    //            string newFolderName = @"Экспорт/";
    //            rootFolder = part.FolderPath;
    //            workFolder = rootFolder + "/" + newFolderName;

    //            // Создать директорию
    //            if (!Directory.Exists(workFolder))
    //                Directory.CreateDirectory(workFolder);

    //            //Отобразить информацию на экране
    //            AppConsole.ShowDocType(part);
    //            AppConsole.ShowDocName(part);
    //            AppConsole.ShowProp(part);

    //            //Экспорт
    //            if ((int)part.DocType == 1 || (int)part.DocType == 2)
    //            {

    //                Console.WriteLine("\nДля сохранения нажмите пробел. Для отмены, другую кнопку");
    //                var userAnswer = Console.ReadKey(true);
    //                //Console.WriteLine("char pressed = " + userAnswer.KeyChar);
    //                if (userAnswer.Key == ConsoleKey.Spacebar)
    //                {
    //                    string savingName = partName + " - " + name;
    //                    AppConsole.SaveDXF(part, workFolder, savingName + ".dxf");

    //                    AppConsole.SaveCopy(part, workFolder, savingName + part.GetFileExtension);

    //                    AppConsole.SavePDF(part, workFolder, savingName + ".pdf");

    //                }
    //            }
    //            else
    //            {
    //                AppConsole.SwitchColor(AppConsole.ColorMode.Warning);
    //                Console.WriteLine("\nСохранение невозможно.");
    //                AppConsole.SwitchColor();
    //            }

    //        }
    //        while (AppConsole.ReloadModel());

    //        Console.WriteLine("Пока-пока");
    //        Thread.Sleep(400);

    //    }
    //}


    ///// <summary>
    ///// Консольный интерфейс
    ///// </summary>
    //public static class AppConsole
    //{

    //    public enum ColorMode
    //    {
    //        Default,
    //        Title,
    //        Warning,
    //        Info
    //    }
    //    public static void SwitchColor(ColorMode colorMode = ColorMode.Default)
    //    {
    //        switch (colorMode)
    //        {
    //            case ColorMode.Default:
    //                Console.BackgroundColor = ConsoleColor.Black;
    //                Console.ForegroundColor = ConsoleColor.White;
    //                break;
    //            case ColorMode.Title:
    //                Console.BackgroundColor = ConsoleColor.Blue;
    //                Console.ForegroundColor = ConsoleColor.White;
    //                break;
    //            case ColorMode.Warning:
    //                Console.BackgroundColor = ConsoleColor.Black;
    //                Console.ForegroundColor = ConsoleColor.Red;
    //                break;
    //            case ColorMode.Info:
    //                Console.BackgroundColor = ConsoleColor.Black;
    //                Console.ForegroundColor = ConsoleColor.Gray;
    //                break;
    //            default:
    //                Console.ResetColor();
    //                break;
    //        }
    //    }

    //    /// <summary>
    //    /// Work with console
    //    /// </summary>
    //    public static class StringManager
    //    {

    //        /// <summary>
    //        /// Очистить строку
    //        /// </summary>
    //        /// <param name="lineNum">Номер строки</param>
    //        public static void ClearLine(int lineNum)
    //        {
    //            Console.CursorTop = lineNum;
    //            Console.CursorLeft = 0;
    //            Console.Write(new string(' ', Console.WindowWidth));
    //            Console.CursorTop = lineNum;
    //        }

    //        /// <summary>
    //        /// Анимация ожидания
    //        /// </summary>
    //        /// <param name="stringNum">Номер строки</param>
    //        /// <param name="processTime">Время ожидания в мс</param>
    //        public static void SleepAnimation(int stringNum = -1, int processTime = 700)
    //        {
    //            int printTime = 100;
    //            int StringLenght = 10;
    //            int repeats = processTime / printTime / StringLenght;

    //            if(stringNum != -1)
    //            {
    //                Console.CursorTop = stringNum;
    //            }

    //            Console.CursorVisible = false; //Выключить курсор
    //            try
    //            {

    //                while (repeats > 0)
    //                {
    //                    for (int i = StringLenght; i > 0; --i)
    //                    {
    //                        Thread.Sleep(printTime);
    //                        Console.Write("- ");
    //                    }
    //                    Thread.Sleep(printTime);
    //                    ClearLine(Console.CursorTop);
    //                    //repeats--;

    //                    Thread.Sleep(printTime);
    //                    ClearLine(Console.CursorTop);

    //                }
    //            }
    //            catch (ThreadInterruptedException e) { }
    //            finally
    //            {
    //                ClearLine(Console.CursorTop);
    //                Console.CursorVisible = true; //Включить курсор
    //            }

    //        }

    //        /// <summary>
    //        /// Название типа детали
    //        /// </summary>
    //        /// <param name="docType"></param>
    //        /// <returns></returns>
    //        public static string DocTypeName(swDocumentTypes_e docType)
    //        {
    //            string typeMessage;
    //            switch (docType)
    //            {
    //                case swDocumentTypes_e.swDocASSEMBLY:
    //                    typeMessage = "Сборка";
    //                    break;
    //                case swDocumentTypes_e.swDocDRAWING:
    //                    typeMessage = "Чертёж";
    //                    break;
    //                case swDocumentTypes_e.swDocPART:
    //                    typeMessage = "Деталь";
    //                    break;
    //                default:
    //                    typeMessage = "Что то непонятное";
    //                    break;
    //            }
    //            return typeMessage;
    //        }

    //    }

    //    public static void Greenings(string appversion = "")
    //    {
    //        string text = $"Welcome to SolidApp {appversion}\n";
    //        Console.Title = "SolidApp ";

    //        SwitchColor(ColorMode.Title);
    //        Console.WriteLine(text);
    //        SwitchColor(ColorMode.Default);

    //    }

    //    /// <summary>
    //    /// Приглашение запустить SW
    //    /// </summary>
    //    /// <param name="sleepTime">Время ожидания</param>
    //    /// <returns></returns>
    //    public static bool RunSW(int sleepTime = 1000)
    //    {
    //        Console.CursorLeft = 0;
    //        Console.CursorTop = 3;
    //        Console.WriteLine("Ожидание запуска SolidWorks");

    //        Thread Animation = new Thread(() => StringManager.SleepAnimation(2, 5000));
    //        Animation.Start(); // Запуск потока с анимацией
    //        while (SwProcess.IsRunning == false)
    //        {
    //            Thread.Sleep(500);
    //            //StringManager.SleepAnimation(stringNum: 2, processTime: sleepTime);
    //        }

    //        StringManager.ClearLine(3);
    //        Animation.Interrupt(); //Прервать поток анимации загрузки
    //        return true;
    //    }

    //    /// <summary>
    //    /// Сообщение о загрузке SW
    //    /// </summary>
    //    /// <returns></returns>
    //    public static bool LoadSwApp()
    //    {
    //        Console.CursorLeft = 0;
    //        Console.CursorTop = 3;
    //        Console.WriteLine("Ожидание загрузки SolidWorks");

    //        Thread Animation = new Thread( () => StringManager.SleepAnimation(2, 5000));
    //        Animation.Start(); // Запуск потока с анимацией

    //        while(SwProcess.swApp is null)
    //        {
    //            Thread.Sleep(500);
    //            //StringManager.SleepAnimation(2);
    //        }
    //        StringManager.ClearLine(3);
    //        Animation.Interrupt(); //Прервать поток анимации загрузки
    //        return true;
    //    }        

    //    /// <summary>
    //    /// Ожидание активного документа
    //    /// </summary>
    //    /// <returns></returns>
    //    public static ModelDoc2 LoadActiveDoc()
    //    {
    //        Console.CursorLeft = 0;
    //        Console.CursorTop = 3;
    //        Console.WriteLine("Откройте деталь");

    //        Thread Animation = new Thread( () => StringManager.SleepAnimation(2, 5000));
    //        Animation.Start(); // Запуск потока с анимацией

    //        ModelDoc2 swModel;
    //        do
    //        {
                
    //            swModel = SwProcess.swApp.ActiveDoc;
    //            Thread.Sleep(500);
    //            //StringManager.SleepAnimation(2);
    //        }
    //        while (swModel is null);

    //        StringManager.ClearLine(3);
    //        Animation.Interrupt(); //Прервать поток анимации загрузки
    //        StringManager.ClearLine(2);
    //        return swModel;
    //    }

    //    /// <summary>
    //    /// Напечатать тип детали
    //    /// </summary>
    //    /// <param name="swModel"></param>
    //    /// <returns></returns>
    //    public static bool ShowDocType(SwModelManager swModelMan)
    //    {
    //        swDocumentTypes_e docType;
    //        bool ret = false;
    //        int lineNum = 3;
    //        Console.CursorLeft = 0;
    //        Console.CursorTop = lineNum;
    //        Console.WriteLine("Открытый документ отсутствует");
    //        if(!(swModelMan is null))
    //        {
    //            docType = swModelMan.DocType;

    //            StringManager.ClearLine(lineNum);
    //            Console.WriteLine("{0, 23} {1}", "Тип открытой детали: ", StringManager.DocTypeName(docType));
    //            ret = true;
    //        }

    //        return ret;
    //    }

    //    /// <summary>
    //    /// Напечатать имя файла
    //    /// </summary>
    //    /// <param name="swModelMan"></param>
    //    /// <returns></returns>
    //    public static bool ShowDocName(SwModelManager swModelMan)
    //    {
    //        bool ret = false;
    //        int lineNum = 4;
    //        Console.CursorLeft = 0;
    //        Console.CursorTop = lineNum;
    //        Console.WriteLine("Отсутствует");
    //        if (!(swModelMan is null))
    //        {
    //            StringManager.ClearLine(lineNum);
    //            Console.WriteLine("{0, 23} {1}", "Имя документа: ", swModelMan.FileName);
    //            ret = true;
    //        }
    //        return ret;
    //    }
    //    /// <summary>
    //    /// Написать список параметров сборки или детали
    //    /// </summary>
    //    /// <param name="swModelMan"></param>
    //    public static void ShowProp(SwModelManager swModelMan)
    //    {
    //        bool ret = false;
    //        const int offset = 23;
    //        int lineNum = 6;
            
    //        Console.CursorTop = lineNum;
    //        swDocumentTypes_e doctype = swModelMan.DocType;
    //        if (doctype == swDocumentTypes_e.swDocPART || doctype == swDocumentTypes_e.swDocASSEMBLY)
    //        {

    //            string NameProp = swModelMan.PrpMan.Title;
    //            string parnNum = swModelMan.PrpMan.GetParam("Обозначение");
    //            string configName = swModelMan.PrpMan.GetActiveConf;
    //            bool isDrawExist = SwFileManager.isDrawExcist(swModelMan.FilePath);
    //            bool isSheet = swModelMan.PrpMan.isSheet;

    //            string DrawIsFound = isDrawExist ? "Найден" : "Не найден";
    //            string isSheetMetal = isSheet ? $"Листовая - {swModelMan.PrpMan.GetSheetThickness}мм" : "Не листовая";

    //            //Clear console below
    //            for (int i = lineNum; i < lineNum + 5; ++i)
    //            {
    //                StringManager.ClearLine(i);
    //            }

    //            Console.CursorLeft = 0;
    //            Console.CursorTop = lineNum;

    //            Console.WriteLine($"{"Обозначение: ",offset} {parnNum}");
    //            Console.WriteLine($"{"Наименование: ",offset} {NameProp}");
    //            Console.WriteLine($"{"Активная конфигурация: ",offset} {configName}");
    //            Console.WriteLine($"{"Одноимённый чертёж: ",offset} {DrawIsFound}");
    //            Console.WriteLine($"{"Тип детали: ",offset} {isSheetMetal}");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"{"Имя чертежа: ",offset} {swModelMan.swModel.GetTitle()}");
    //        }
    //    }


    //    public static bool ReloadModel()
    //    {
    //        bool ret;
    //        ConsoleKeyInfo userAnswer;
    //        int lineNum = 15;
    //        Console.CursorTop = lineNum;
    //        StringManager.ClearLine(lineNum);
    //        Console.CursorLeft = 0;

    //        Console.WriteLine("Для выхода нажмите Esc, перезагрузка модели - пробел");
    //        userAnswer = Console.ReadKey(true);

    //        switch (userAnswer.Key)
    //        {
    //            case ConsoleKey.Spacebar:
    //                ret = true;
    //                break;
    //            case ConsoleKey.Escape:
    //                ret = false;
    //                break;
    //            default:
    //                ret = true;
    //                break;
    //        }

    //        for(int i = lineNum; i >= 1; --i)
    //        {
    //            StringManager.ClearLine(i);
    //        }

    //        return ret;
    //    }

    //    /// <summary>
    //    /// Сохранить развёртку
    //    /// </summary>
    //    /// <param name="swModelMan"></param>
    //    /// <param name="folder">Папка сохранения</param>
    //    /// <param name="name">Имя файла с расширением</param>
    //    /// <returns></returns>
    //    public static bool SaveDXF(SwModelManager swModelMan, string folder = null, string name = null)
    //    {
    //        bool ret = false;
    //        int lineNum = 12;
    //        const int offset = 23;
    //        Console.CursorLeft = 0;
    //        //Console.CursorTop = lineNum;

    //        if (string.IsNullOrEmpty(folder))
    //            folder = swModelMan.FolderPath;

    //        if (string.IsNullOrEmpty(name))
    //            name = swModelMan.FileNameWhithoutExt + ".dxf";


    //        StringManager.ClearLine(lineNum);
    //        Console.Write($"{"Сохранение DXF: ",offset}");

    //        if (swModelMan.PrpMan.isSheet)
    //        {
    //            ret = swModelMan.Export.SaveDxf(folder + name);
    //        }

    //        Console.Write(ret ? " OK\n" : " Не сохранено\n");
    //        return ret;
    //    }
    //    /// <summary>
    //    /// Сохранить копию документа
    //    /// </summary>
    //    /// <param name="swModelMan"></param>
    //    /// <param name="folder">Папка сохранения</param>
    //    /// <param name="name">Имя файла с расширением</param>
    //    /// <returns></returns>
    //    public static bool SaveCopy(SwModelManager swModelMan, string folder = null, string name = null)
    //    {
    //        bool ret = false;
    //        int lineNum = 13;
    //        const int offset = 23;
    //        Console.CursorLeft = 0;

    //        if (string.IsNullOrEmpty(folder))
    //            folder = swModelMan.FolderPath;

    //        if (string.IsNullOrEmpty(name))
    //            name = swModelMan.FileName;


    //        StringManager.ClearLine(lineNum);
    //        Console.Write($"{"Сохранение копии: ",offset}");

    //        ret = swModelMan.SaveAsCopy(folder + name);

    //        Console.Write(ret ? " OK\n" : " Не сохранено\n");
    //        return ret;
    //    }

    //    /// <summary>
    //    /// Сохранить PDF из чертежа с тем же именем
    //    /// </summary>
    //    /// <param name="swModelMan"></param>
    //    /// <param name="folder">Папка сохранения</param>
    //    /// <param name="name">Имя файла с расширением</param>
    //    /// <returns></returns>
    //    public static bool SavePDF(SwModelManager swModelMan, string folder = null, string name = null)
    //    {
    //        bool ret = false;
    //        int lineNum = 14;
    //        const int offset = 23;
    //        Console.CursorLeft = 0;

    //        if (string.IsNullOrEmpty(folder))
    //            folder = swModelMan.FolderPath;

    //        if (string.IsNullOrEmpty(name))
    //            name = swModelMan.FileNameWhithoutExt + ".pdf";


    //        StringManager.ClearLine(lineNum);
    //        Console.Write($"{"Сохранение PDF: ",offset}");

    //        DrawingDoc DrawDoc;

    //        string drawName = Path.ChangeExtension(swModelMan.FileName, "SLDDRW");
    //        var openedDraw = SwProcess.swApp.GetOpenDocument(drawName);
    //        bool docIsOpened = false;
    //        if (!(openedDraw is null))
    //            docIsOpened = true;

    //        if (SwFileManager.OpenDraw(swModelMan.FilePath, out DrawDoc))
    //        {
    //            var model = (ModelDoc2)DrawDoc;
                
    //            ret = swModelMan.Export.SavePdf(DrawDoc, folder + name, true);
                
    //            if(!docIsOpened)
    //                SwProcess.swApp.CloseDoc(model.GetTitle());
    //        }

    //        Console.Write(ret ? " OK\n" : " Не сохранено\n");
    //        return ret;
    //    }



    //}
}
