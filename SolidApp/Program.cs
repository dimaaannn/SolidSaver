using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SWAPIlib;
using System.Collections;
using SWAPIlib.ComConn;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.Global;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidApp.SW;
using Microsoft.Extensions.Logging;

namespace SolidApp
{
    class Program
    {
        private static ILogger _logger;
        private static ISwConnector _swConnector;

        //Статический конструктор вызывается каждый раз при запуске (обращении к классу)
        static Program()
        {
            InitDI(); //Инициализировать DependencyInjection (обязательно!)
        }
        static void Main(string[] args)
        {
            _logger.LogInformation("Подключение к процессу SolidWorks");
            _swConnector.Connect();

            //пример взаимодействия с стандартной библиотекой SolidWorks
            if (_swConnector.IsComConnected)                            //Проверить что подключение установлено
            {
                ISldWorks sldWorksApp = _swConnector.SwApp;             //получить основной объект приложения SolidWorks
                IModelDoc2 activeModel = sldWorksApp.IActiveDoc2;       //Получить открытый документ
                LogModelInfo(activeModel);                              //Вывести информацию о документе

                if(activeModel is AssemblyDoc assemblyDocument)
                {
                    bool topLevelOnly = true;                           //Выводить только список элементов верхнего уровня сборки

                    //Получить компоненты сборки (компонент - обёртка для модели)
                    //Потребовалась дополнительная функция, потому что метод библиотеки реализован криво
                    Component2[] assemblyComponents = GetAssemblyComponents(assemblyDocument, topLevelOnly); 

                    Console.WriteLine("\nСписок компонентов сборки:\n");
                    foreach (Component2 component in assemblyComponents)
                    {
                        Console.WriteLine("Имя компонента: " + component.Name);                     //Напечатать имя компонента

                        ModelDoc2 modelInComponent = component.GetModelDoc2();                      //Получить объект модели внутри компонента
                        Console.WriteLine(
                            "Конфигурация: " + modelInComponent?.IGetActiveConfiguration().Name);   //Имя активной конфигурации
                        Console.WriteLine("Путь к файлу: "+ modelInComponent?.GetPathName());       //Путь к файлу
                    }
                }
    
            }
            else
            {
                _logger.LogWarning("Подключение не удалось");           //Вывести сообщение об ошибке
            }

            Console.ReadKey();
        }

        public static void InitDI()
        {
            Startup.Init(null);
            //Получить объект логгера
            _logger = Startup.ServiceProvider.GetService<ILogger<Program>>();
            //Получить экземпляр класса для подключения к SW
            _swConnector = Startup.ServiceProvider.GetService<ISwConnector>();
        }

        public static void LogModelInfo(IModelDoc2 swModel)
        {
            SwConst.swDocumentTypes_e docType;
            string activeModelTitle;

            if(swModel != null)
            {
                //Преобразовать тип документа в читабельный вид (Enum)
                docType = (SwConst.swDocumentTypes_e)swModel.GetType();
                //Получить имя модели
                activeModelTitle = swModel.GetTitle();
                _logger.LogInformation($"Имя активной модели: {activeModelTitle}, тип: {docType}");
            }
            else
            {
                _logger.LogWarning("Открытый документ отсутствует");
            }
        }

        public static Component2[] GetAssemblyComponents(AssemblyDoc swAssembly, bool topLevelOnly = true)
        {
            object[] components = swAssembly.GetComponents(topLevelOnly);
            return components.Cast<Component2>().ToArray();
        }

        /// <summary>
        /// Старый интерфейс
        /// </summary>
        public static void SWLibTest()
        {
            SwAppControl.Connect();

            //Get Raw document
            var appmodel = ModelClassFactory.ActiveDoc;

            //Print root title
            AppAssembly rootAsm = null;
            ModelDoc2 testRawModel = null;
            Component2 TestRawComponent = null;
            List<IAppComponent> compList = null;
            if (appmodel.SwModel is AssemblyDoc swAsm)
            {
                rootAsm = new AppAssembly(appmodel.SwModel);
                Console.WriteLine($"RootDoc: {rootAsm.Title}");
                compList = rootAsm.GetComponents(true);
                testRawModel = compList.First().SwModel;
                TestRawComponent = compList.First().SwCompModel;
            }
        }
    }


    

    public class NotifyOnInstance
    {
        public NotifyOnInstance()
        {
            Console.WriteLine("Class created");
        }
        public NotifyOnInstance(string s)
        {
            Console.WriteLine($"Class created with {s}");
        }
    }

}
