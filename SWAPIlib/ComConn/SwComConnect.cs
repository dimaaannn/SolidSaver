using System;
using System.Diagnostics;
using System.Linq;
using SolidWorks.Interop.sldworks;
using SWAPIlib.ComConn.Proxy;

namespace SWAPIlib.ComConn
{
    public static class SwAppControl
    {
        public static bool Interface; //TODO создать интерфейс для подключения

        private static bool _comStatus = false;
        public static bool ComConnected { get => _comStatus; }

        public static ISldWorks swApp { get => SwAPI.swApp; }

        private static ModelDoc2 _mainModel = null;
        /// <summary>
        /// Установить базовую модель
        /// </summary>
        public static ModelDoc2 MainModel
        {
            get
            {
                ModelDoc2 ret = null;
                if (_mainModel == null && ComConnected)
                {
                    ret = SwAPI.swApp.ActiveDoc;

                    if (ret != null)
                    {
                        string evT = $"Модель {ret?.GetTitle() ?? "не"} установлена";
                        SwEventArgs eventArgs = new SwEventArgs(evT);
                        _mainModel = ret;
                        MainModelChanged?.Invoke(_mainModel, eventArgs);
                        //TODO обработать событие DestroyNotify2 при закрытии
                    }
                }
                return ret;
            }
            set
            {
                _mainModel = value;
                string evT = $"Модель {_mainModel?.GetTitle() ?? "не"} установлена";
                SwEventArgs eventArgs = new SwEventArgs(evT);
                MainModelChanged?.Invoke(_mainModel, eventArgs);
            }
        }

        /// <summary>
        /// Загрузить активную модель
        /// </summary>
        public static ModelDoc2 ActiveModel
        {
            get
            {
                ModelDoc2 ret = null;
                if (ComConnected)
                {
                    ret = SwAPI.swApp.ActiveDoc;
                    string evT = $"Модель {ret?.GetTitle() ?? "не"} получена";
                }
                return ret;
            }
        }

        /// <summary>
        /// Обнаружение процесса SW
        /// </summary>
        public static event EventHandler SwProcessRunning
        {
            add { SwAPI.SwIsRunning += value; }
            remove { SwAPI.SwIsRunning -= value; }
        }

        /// <summary>
        /// Изменение базовой модели
        /// </summary>
        public static event EventHandler MainModelChanged;

        /// <summary>
        /// Подключиться к COM
        /// </summary>
        /// <returns></returns>
        public static void Connect()
        {
            SwAPI.ComConnected += SwAPI_ComConnected;
            SwAPI.SwIsDisposed += SwAPI_SwIsDisposed;

            var swApp = SwAPI.swApp;
            while (swApp == null)
            {
                System.Threading.Thread.Sleep(500);
                swApp = SwAPI.swApp;
            }
        }

        /// <summary>
        /// Событие при закрытии SW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SwAPI_SwIsDisposed(object sender, EventArgs e)
        {
            _comStatus = false;
            _mainModel = null;
        }

        /// <summary>
        /// Событие при подключении к com
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SwAPI_ComConnected(object sender, EventArgs e)
        {
            _comStatus = true;
        }

    }

}
namespace SWAPIlib.ComConn.Proxy
{
    /// <summary>
    /// Основные операции с API
    /// </summary>
    public static class SwAPI
    {
        private static Process _swProcess;
        private static ISldWorks _swApp;

        /// <summary>
        /// Процесс SolidWorks найден
        /// </summary>
        public static event EventHandler SwIsRunning;
        public static event EventHandler SwIsDisposed
        {
            add => swProcess.Disposed += value;
            remove => swProcess.Disposed -= value;
        }
        public static event EventHandler ComConnected;


        /// <summary>
        /// GetSolidWorks process
        /// </summary>
        public static Process swProcess
        {
            get
            {
                if (_swProcess is null)
                {
                    Process[] ProcessList;
                    ProcessList = Process.GetProcessesByName("SLDWORKS");
                    if (ProcessList.Count() > 0)
                    {
                        _swProcess = ProcessList.First();
                        _swProcess.EnableRaisingEvents = true;
                        string evText = "SolidWorks process is running";
                        SwIsRunning?.Invoke(_swProcess, new SwEventArgs(evText));
                        Debug.WriteLine(evText);
                    }
                }
                return _swProcess;
            }
        }

        /// <summary>
        /// Check SolidWorks is running
        /// </summary>
        public static bool IsRunning
        {
            get
            {
                bool ret = false;
                if (_swProcess != null)
                    ret = true;
                return ret;
            }

        }

        /// <summary>
        /// Подключение к com API
        /// </summary>
        /// <returns></returns>
        private static ISldWorks GetSWApp()
        {
            string progId = "SldWorks.Application";
            var progType = Type.GetTypeFromProgID(progId);
            ISldWorks swApp = null;

            Debug.Print("geting SWapp");
            string evText = "Sw API connected";
            swApp = Activator.CreateInstance(progType) as ISldWorks;

            if (swApp != null)
            {
                Debug.WriteLine(evText);
                ComConnected?.Invoke(swApp, new SwEventArgs(evText));
            }
            return swApp;
        }

        /// <summary>
        /// Получить экземпляр АПИ
        /// </summary>
        public static ISldWorks swApp
        {
            get
            {
                if (_swApp is null)
                    _swApp = GetSWApp();
                return _swApp;
            }

        }
    }

    /// <summary>
    /// Аргументы события
    /// </summary>
    public class SwEventArgs : EventArgs
    {
        public string Text;

        public SwEventArgs(string EventText) =>
            Text = EventText;
        public SwEventArgs() => Text = null;
    }
}
