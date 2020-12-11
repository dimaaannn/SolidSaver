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
        public virtual AppDocType DocType { get; }
        public string FileName { get => System.IO.Path.GetFileName(Path); }
        public virtual string Title { get => ModelProxy.GetName(_swModel); }
        public string Path { get; }
        public List<ISwProperty> PropList { get; protected set; }
        public IFileModelProp GlobalModelProp { get; }

        public event EventHandler<SwEventArgs> CloseFile;

        /// <summary>
        /// ModelDoc2 Constructor
        /// </summary>
        /// <param name="swModel"></param>
        public AppModel(ModelDoc2 swModel)
        {
                PropList = new List<ISwProperty>();
                GlobalModelProp = new FileModelProp(this)
                { 
                    IsRoot = false,
                };
                IsExist = true;
                DocType = PartTypeChecker.GetSWType(swModel);

            if (swModel != null)
            {
                //TODO Add default properties to list
                this._swModel = swModel;
                Path = ModelProxy.GetPathName(SwModel);


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
                string msg = "new Model- null reference";
                Debug.WriteLine(msg);
                //throw new NullReferenceException(msg);
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

        /// <summary>
        /// Список имён конфигураций
        /// </summary>
        public virtual List<string> ConfigList
        {
            get
            {
                if(_configList == null)
                    _configList = new List<string>(SwModel.GetConfigurationNames());
                return _configList;
            }
        }
        private List<string> _configList;

        public virtual bool VisibState
        {
            get => SwModel.Visible;
            set => SwModel.Visible = value;
        }
    }


    public class AppAssembly: AppModel, ISwAssembly
    {
        public AppAssembly(ModelDoc2 swModel) : base(swModel) 
        {
            _swAsm = swModel as AssemblyDoc;
            PropList = new List<ISwProperty>();
            PropList.AddRange(PropertyFactory.ModelProperty.DefaultModelProp(this));
        }
        private AssemblyDoc _swAsm;
        public string ConfigName 
        { 
            get => ModelConfigProxy.GetActiveConfName(SwModel);
            set => ModelConfigProxy.SetActiveConf(SwModel, value);
        }

        

        public int ComponentCount(bool TopLevelOnly = true)
        {
            return AsmDocProxy.GetComponentCount(SwModel, TopLevelOnly);
        }

        public List<IAppComponent> GetComponents(bool TopLevelOnly)
        {
            var ret = new List<IAppComponent>();
            var swComponents = AsmDocProxy.GetComponents(SwModel, TopLevelOnly);
            foreach(Component2 comp in swComponents)
            {
                    ret.Add(new AppComponent(comp));
            }
            return  ret ;
        }
    }

    /// <summary>
    /// Фабрика типов модели
    /// </summary>
    public static class ModelFactory
    {
        public static AppModel GetModel(ModelDoc2 swModel)
        {
            AppDocType docType = PartTypeChecker.GetSWType(swModel);
            AppModel ret = null;
            switch (docType)
            {
                case AppDocType.swNONE:
                    break;
                case AppDocType.swASM:
                    ret = new AppAssembly(swModel);
                    break;
                default:
                    ret = new AppModel(swModel);
                    break;
            }
            return ret;
        }

        public static AppModel ActiveDoc
        {
            get => GetModel(SwAppControl.ActiveModel);
        }
    }


    /// <summary>
    /// Класс компонента
    /// </summary>
    public class SwComponent : AppModel, ISwComponent
    {
        public Component2 SwComp { get; private set; }
        public override AppDocType DocType { get => _docType; }
        private AppDocType _docType;
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
        public override string Title => ComponentProxy.GetName(SwComp);
        public string ConfigName
        {
            get => ComponentProxy.GetRefConfig(SwComp);
            set => ComponentProxy.SetRefConfig(SwComp, value);
        }
        /// <summary>
        /// Component Constructor
        /// </summary>
        /// <param name="component"></param>
        public SwComponent(Component2 component): base(ComponentProxy.GetModelDoc2(component))
        {
            SwComp = component;
            if (SwModel != null)
                _docType = PartTypeChecker.GetSWType(SwModel);
            else _docType = AppDocType.swCOMPONENT;

            var abc = new List<int>();
            
            //PropList.AddRange( PropSheetTemplate.Component(this));
        }
    }

    //TODO add ConfigName
    //public class AppPart  : ISwPart
    //{
    //    int BodyCount { get; }
    //    bool IsSheetMetal { get; }
    //    bool HasDrawing { get; }
    //    bool SheetThickness(int BodyIndex);
    //    bool ExportDXF(string path);
    //    bool ExportModel(string path);
    //    bool ExportDrawing(string path);
    //}
}
