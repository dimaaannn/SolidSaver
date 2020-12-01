using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SwConst;
using System.Diagnostics;



namespace SWAPIlib
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
        /// Обнаружение процесса SW
        /// </summary>
        public static event System.EventHandler SwProcessRunning
        {
            add { SwAPI.SwIsRunning += value; }
            remove { SwAPI.SwIsRunning -= value; }
        }

        /// <summary>
        /// Изменение базовой модели
        /// </summary>
        public static event System.EventHandler MainModelChanged;

        /// <summary>
        /// Подключиться к COM
        /// </summary>
        /// <returns></returns>
        public static void Connect()
        {
            SwAPI.ComConnected += SwAPI_ComConnected;
            SwAPI.SwIsDisposed += SwAPI_SwIsDisposed;

            var swApp = SwAPI.swApp;
            while(swApp == null)
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


    
    public interface ISwPartData
    {
        string PartType { get; }
        string FileName { get; }
        bool IsSelected { get; }

    }

    public class PartParamGetter
    {
        public ModelDoc2 SwModel { get; set; }
        Dictionary<string, string> paramsDict;
        public Dictionary<string, string> ParamsDict 
        { 
            get => paramsDict; 
            set => paramsDict = value; 
        }

    }

    /// <summary>
    /// Базовый класс модели
    /// </summary>
    public class AppModel : ISwModel
    {
        private ModelDoc2 _swModel;

        public bool IsExist { get; private set; }
        public ModelDoc2 SwModel { get => _swModel; }
        public AppDocType DocType { get; }
        public string FileName { get => System.IO.Path.GetFileName(Path); }
        public virtual string Title { get => ModelProxy.GetName(_swModel); }
        public string Path { get; }

        public event EventHandler<SwEventArgs> CloseFile;

        /// <summary>
        /// ModelDoc2 Constructor
        /// </summary>
        /// <param name="swModel"></param>
        public AppModel(ModelDoc2 swModel)
        {
            if (swModel != null)
            {
                this._swModel = swModel;
                DocType = PartTypeChecker.GetSWType(swModel);
                Path = ModelProxy.GetPathName(SwModel);
                IsExist = true;

                #region EventProxy
                if (DocType == AppDocType.swASM)
                {
                    (swModel as AssemblyDoc).DestroyNotify += CloseFileHandler;
                }
                else if(DocType == AppDocType.swPART)
                {
                    (swModel as PartDoc).DestroyNotify += CloseFileHandler;
                }
                else if (DocType == AppDocType.swDRAWING)
                {
                    (swModel as DrawingDoc).DestroyNotify += CloseFileHandler;
                }
                #endregion
            }
            else
            {
                string msg = "new Model- wrong reference";
                Debug.WriteLine(msg);
                throw new NullReferenceException("msg");
            }
            string succes = $"Class Model created - {DocType} - {FileName}";

        }

        private int CloseFileHandler()
        {
            IsExist = false;
            string evT = $"Document {FileName} closed";
            var evArg = new SwEventArgs(evT);
            CloseFile?.Invoke(this, evArg);
            return 0;
        }
    }

    public class SwProperty //: ISwProperty
    {
        public AppModel AppModel { get; set; }
        public bool IsReadable { get; }
        public bool IsWritable { get; }

        public string UserName { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        public void Update() { }
        public bool WriteValue() 
        {
            return false;
        }
    }

    /// <summary>
    /// Класс компонента
    /// </summary>
    public class SwComponent : AppModel, ISwComponent
    {
        public Component2 SwComp { get; private set; }
        public AppSuppressionState SuppressionStatus 
            { get => PartTypeChecker.GetAppSuppressionState(SwComp); }
        public int ComponentCount { get => SwComp.IGetChildrenCount(); }
        public List<SwComponent> GetComponents(bool TopLeverOnly)
        {
            var ret = new List<SwComponent>();
            var components = ComponentProxy.GetChildren(SwComp);
            Debug.WriteLine($"GetComponents from {this.FileName} begin");
            foreach(object comp in components)
            {
                ret.Add(new SwComponent(comp as Component2));
            }
            Debug.WriteLine(ret.Count > 0 ? "Success" : "No components");
            
            return ret;
        }
        public SwComponent GetRootComponent()
        {
            return new SwComponent(ComponentProxy.GetRoot(SwComp));
        }

        public SwComponent(Component2 component): base(ComponentProxy.GetModelDoc2(component))
        {
            SwComp = component;
        }
    }
}
