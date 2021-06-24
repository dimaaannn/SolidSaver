using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SWAPIlib.ComConn.Proxy;

namespace SWAPIlib.ComConn
{
    public static class SwAppControl
    {
        static SwAppControl()
        {
            SwAPI.ComConnected += SwAPI_ComConnected;
            SwAPI.SwIsDisposed += SwAPI_SwIsDisposed;
        }
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
                    ret = ActiveModel;

                    if (ret != null)
                    {
                        string evT = $"Модель {ret?.GetTitle() ?? "не"} установлена";
                        SwEventArgs eventArgs = new SwEventArgs(evT);
                        _mainModel = ret;
                        MainModelChanged?.Invoke(_mainModel, eventArgs);
                        LoadMainModelAction(_mainModel);
                        _mainModel = ret;
                    }
                }
                return _mainModel;
            }
            set
            {
                ChangeMainModelAction(_mainModel); //Unsubscribe events

                _mainModel = value;

                string evT = $"Модель {_mainModel?.GetTitle() ?? "не"} установлена";
                LoadMainModelAction(_mainModel); // Subscribe
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
            

            var swApp = SwAPI.swApp;
            while (swApp == null)
            {
                System.Threading.Thread.Sleep(500);
                swApp = SwAPI.swApp;
            }
        }

        private static void LoadMainModelAction(ModelDoc2 model)
        {
            if(model is AssemblyDoc asm)
            {
                asm.DestroyNotify2 += RootModelClose;
                asm.NewSelectionNotify += Asm_NewSelectionNotify;
            }
            if(model is PartDoc part)
            {
                part.DestroyNotify2 += RootModelClose;
            }
        }
        private static void ChangeMainModelAction(ModelDoc2 model)
        {
            if (model is AssemblyDoc asm)
            {
                asm.DestroyNotify2 -= RootModelClose;
                asm.NewSelectionNotify -= Asm_NewSelectionNotify;
            }
            if (model is PartDoc part)
            {
                part.DestroyNotify2 -= RootModelClose;
            }
        }

        #region Events

        private static int Asm_NewSelectionNotify()
        {
            SelectionNotify?.Invoke(MainModel, new EventArgs());
            return 0;
        }
        /// <summary>
        /// Изменено выделение
        /// </summary>
        public static event EventHandler SelectionNotify;
        private static int RootModelClose(int DestroyType)
        {
            RootModelClosed?.Invoke(_mainModel, new EventArgs());
            return DestroyType;
        }
        /// <summary>
        /// Основная модель закрыта
        /// </summary>
        public static event EventHandler RootModelClosed;
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
        #endregion

        /// <summary>
        /// Передать все загруженные в память модели в метод
        /// </summary>
        /// <param name="modelAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task GetOpenedModels(
            Action<ModelDoc2> modelAction,
            CancellationToken cancellationToken)
        {
            var enumOpenedDocuments = swApp.EnumDocuments2();

            ModelDoc2 model = await GetNextDocAsync(enumOpenedDocuments);

            while(model != null &&
                cancellationToken.IsCancellationRequested == false)
            {
                modelAction(model);    
                model = await GetNextDocAsync(enumOpenedDocuments);
            }
        }

        private static async Task<ModelDoc2> GetNextDocAsync(EnumDocuments2 enumerator)
        {
            return await System.Threading.Tasks.Task<ModelDoc2>.Run(() =>
            {
                int fetched = 0;
                ModelDoc2 model;
                enumerator.Next(1, out model, ref fetched);
                return model;
            });
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
