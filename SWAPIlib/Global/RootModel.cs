using System;
using System.Collections.Generic;
using System.ComponentModel;
using SWAPIlib.ComConn;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn.Proxy;

namespace SWAPIlib.Global
{

    public interface IRootModel : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Коренная модель
        /// </summary>
        AppModel appModel { get; set; }
        /// <summary>
        /// Тип документа модели
        /// </summary>
        AppDocType DocType { get; }
        /// <summary>
        /// Имя документа
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        string Path { get; }
        /// <summary>
        /// Имя активной конфигурации
        /// </summary>
        string ActiveConfigName { get; set; }

        /// <summary>
        /// Активный список свойств
        /// </summary>
        IList<ISwProperty> ActivePropList { get; }
        /// <summary>
        /// Класс обработки имени проекта
        /// </summary>
        IFileModelProp ProjectNameProp { get; }
        /// <summary>
        /// Дочерние компоненты сборки
        /// </summary>
        List<IAppComponent> SubComponents { get; }

        /// <summary>
        /// Загрузить активную модель
        /// </summary>
        /// <param name="pathToModel"></param>
        /// <returns></returns>
        bool GetMainModel(string pathToModel = null);
        /// <summary>
        /// Загружать компоненты только верхнего уровня
        /// </summary>
        bool TopLevelOnly { get; set; }
        /// <summary>
        /// Загрузить список дочерних компонентов
        /// </summary>
        /// <returns></returns>
        bool GetSubComponents();
        bool LoadActiveModel();

        event EventHandler<SwEventArgs> CloseRootModel;

    }


    public class RootModel : IRootModel
    {
        public AppModel appModel
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
                if (appModel is null)
                    return AppDocType.swNONE;
                else
                {
                    return appModel.DocType;
                }
            }
        }
        /// <summary>
        /// Является ли модель сборкой
        /// </summary>
        private bool IsAssembly => DocType == AppDocType.swASM;

        public string Title => appModel?.Title; //TODO add override for types
        public string Path => appModel?.Path;
        public IList<ISwProperty> ActivePropList { get => appModel?.PropList; }

        public List<IAppComponent> SubComponents { get => _subComponents; }
        private List<IAppComponent> _subComponents;

        public ISwProperty MainInfoProp { get; set; }
        public string ActiveConfigName //TODO добавить класс детали
        {
            get
            {
                if (appModel is AppAssembly assembly)
                    return assembly.ConfigName;
                else return "Отсутствует";
            }
            set
            {
                if (appModel is AppAssembly assembly)
                    assembly.ConfigName = value;
            }
        }

        public IFileModelProp ProjectNameProp { get; set; }

        public bool GetMainModel(string pathToModel = null)
        {
            bool ret = false;
            if (string.IsNullOrEmpty(pathToModel))
            {
                appModel = ModelClassFactory.GetModel(SwAppControl.MainModel);
            }
            return ret;
        }

        public bool LoadActiveModel()
        {
            bool ret = false;
            appModel = ModelClassFactory.ActiveDoc;
            if (appModel != null) ret = true;
            return ret;
        }

        public bool TopLevelOnly { get; set; }
        public bool GetSubComponents()
        {
            bool ret = false;
            if (appModel is IAppAssembly swAssembly)
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
}
