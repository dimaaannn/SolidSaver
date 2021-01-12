using SWAPIlib.VisualInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using SWAPIlib.ComConn;
using SWAPIlib.BaseTypes;

namespace SWAPIlib
{
    public static class GlobalOptions
    {
        /// <summary>
        /// Путь к основной открытой детали
        /// </summary>
        public static string MainPartPath
        {
            get
            {
                if (String.IsNullOrEmpty(_mainPartPath))
                    _mainPartPath = ModelProxy.GetPathName(SwAppControl.MainModel);
                return _mainPartPath;
            }
            set
            {
                _mainPartPath = value;
            }
        }
        static string _mainPartPath;
        /// <summary>
        /// Рабочая папка сборки
        /// </summary>
        public static string ModelRootFolder
        {
            get => String.IsNullOrEmpty(_modelRootFolder) ?
                System.IO.Path.GetDirectoryName(MainPartPath) :
                _modelRootFolder;
            set => _modelRootFolder = value;
        }
        static string _modelRootFolder;
        /// <summary>
        /// Рабочая папка проекта
        /// </summary>
        public static string MainFolder { get; set; } = ModelRootFolder;


    }

    public class MainModel : VisualInterface.IMainModel
    {
        public AppModel RootModel
        {
            get => _RootModel;
            set
            {
                SubComponents?.Clear();
                _RootModel = value;
            }
        }
        private AppModel _RootModel;
        public AppDocType DocType
        {
            get
            {
                if (RootModel is null)
                    return AppDocType.swNONE;
                else
                {
                    return RootModel.DocType;
                }
            }
        }
        /// <summary>
        /// Является ли модель сборкой
        /// </summary>
        private bool IsAssembly => DocType == AppDocType.swASM;

        public string Title => RootModel?.Title; //TODO add override for types
        public string Path => RootModel?.Path;
        public IList<ISwProperty> ActivePropList { get => RootModel?.PropList; }
        public IFileModelProp GlobalModelProp { get => RootModel?.GlobalModelProp; }

        public List<IAppComponent> SubComponents { get => _subComponents; }
        private List<IAppComponent> _subComponents;

        public ISwProperty MainInfoProp { get; set; }
        public string ActiveConfigName //TODO добавить класс детали
        {
            get
            {
                if (RootModel is AppAssembly assembly)
                    return assembly.ConfigName;
                else return "Отсутствует";
            }
            set
            {
                if (RootModel is AppAssembly assembly)
                    assembly.ConfigName = value;
            }
        }

        public IFileModelProp ProjectNameProp { get; set; }

        public bool GetMainModel(string pathToModel = null)
        {
            bool ret = false;
            if (String.IsNullOrEmpty(pathToModel))
            {
                RootModel = ModelFactory.GetModel(SwAppControl.MainModel);
            }
            return ret;
        }

        public bool LoadActiveModel()
        {
            bool ret = false;
            RootModel = ModelFactory.ActiveDoc;
            if (RootModel != null) ret = true;
            return ret;
        }

        public bool TopLevelOnly { get; set; }
        public bool GetSubComponents()
        {
            bool ret = false;
            if (RootModel is ISwAssembly swAssembly)
            {
                _subComponents = swAssembly.GetComponents(TopLevelOnly);
                if (_subComponents.Count > 0)
                    ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Изменение свойства
        /// </summary>
        /// <param name="s"></param>
        protected void RaisePropertyChanged(string s)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(s));
        }

        public event EventHandler<SwEventArgs> CloseRootModel;
        public event PropertyChangedEventHandler PropertyChanged;


    }

    //public class MainModel : VisualInterface.IMainModel, 
    //    INotifyPropertyChanged
    //{
    //    public AppModel RootModel
    //    {
    //        get => _RootModel;
    //        set
    //        {
    //            SubComponents?.Clear();
    //            SubComponents2.Clear();
    //            _RootModel = value;
    //        }
    //    }
    //    private AppModel _RootModel;
    //    public AppDocType DocType 
    //    { get
    //        {
    //            if (RootModel is null)
    //                return AppDocType.swNONE;
    //            else
    //            {
    //                return RootModel.DocType;
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// Является ли модель сборкой
    //    /// </summary>
    //    private bool IsAssembly => DocType == AppDocType.swASM;

    //    public string Title => RootModel?.Title; //TODO add override for types
    //    public string Path => RootModel?.Path;
    //    public IList<ISwProperty> ActivePropList { get => RootModel?.PropList; }
    //    public IFileModelProp GlobalModelProp { get => RootModel?.GlobalModelProp; }

    //    public List<IAppComponent> SubComponents { get => _subComponents; }
    //    public ObservableCollection<IAppComponent> SubComponents2 { get => _subComponents2; }
    //    private List<IAppComponent> _subComponents;
    //    private ObservableCollection<IAppComponent> _subComponents2 = new ObservableCollection<IAppComponent>();

    //    public ISwProperty MainInfoProp { get; set; }
    //    public string ActiveConfigName //TODO добавить класс детали
    //    { 
    //        get
    //        {
    //            if (RootModel is AppAssembly assembly)
    //                return assembly.ConfigName;
    //            else return "Отсутствует";
    //        }
    //        set
    //        {
    //            if (RootModel is AppAssembly assembly)
    //                assembly.ConfigName = value;
    //        }
    //    }

    //    public IFileModelProp ProjectNameProp { get; set; }

    //    public bool GetMainModel(string pathToModel = null)
    //    {
    //        bool ret = false;
    //        if (String.IsNullOrEmpty(pathToModel))
    //        {
    //            RootModel = ModelFactory.GetModel(SwAppControl.MainModel);
    //        }
    //        return ret;
    //    }

    //    public bool LoadActiveModel()
    //    {
    //        bool ret = false;
    //        RootModel = ModelFactory.ActiveDoc;
    //        if (RootModel != null) ret = true;
    //        return ret;
    //    }

    //    public bool TopLevelOnly { get; set; }
    //    public bool GetSubComponents()
    //    {
    //        bool ret = false;
    //        if(RootModel is ISwAssembly swAssembly)
    //        {
    //            _subComponents = swAssembly.GetComponents(TopLevelOnly);
    //            foreach (IAppComponent item in _subComponents)
    //            {
    //                _subComponents2.Add(item);
    //            }
    //            if (_subComponents.Count > 0)
    //                ret = true;
    //        }
    //        return ret;
    //    }

    //    /// <summary>
    //    /// Изменение свойства
    //    /// </summary>
    //    /// <param name="s"></param>
    //    protected void RaisePropertyChanged(string s)
    //    {
    //        if (PropertyChanged != null)
    //            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(s));
    //    }

    //    public event EventHandler<SwEventArgs> CloseRootModel;
    //    public event PropertyChangedEventHandler PropertyChanged;

        
    //}
}
