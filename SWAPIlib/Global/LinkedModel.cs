using System;
using System.Collections.Generic;
using System.ComponentModel;
using SWAPIlib.ComConn;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.MProperty;
using SWAPIlib.Controller;
using System.Collections;
using System.Collections.ObjectModel;
using SolidWorks.Interop.sldworks;

namespace SWAPIlib.Global
{

    public interface ILinkedModel : System.ComponentModel.INotifyPropertyChanged
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
        IList<IPropertyModel> ActivePropList { get; }
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
        /// <summary>
        /// Загрузить активную модель
        /// </summary>
        /// <returns></returns>
        bool LoadModel();
        /// <summary>
        /// Загрузить модель COM
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool LoadModel(ModelDoc2 model);
        /// <summary>
        /// Загрузить модель по ссылке
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool LoadModel(string path);
        /// <summary>
        /// Установить текущую модель в качестве главной
        /// </summary>
        /// <returns></returns>
        bool SetCurrentModelAsMain();

        event EventHandler<SwEventArgs> CloseRootModel;

    }




    public class LinkedModel : ILinkedModel
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
        public IList<IPropertyModel> ActivePropList { get => appModel?.PropList; }

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
            //TODO add open doc by uri
            return ret;
        }

        /// <summary>
        /// Загрузить активную модель
        /// </summary>
        /// <returns></returns>
        public bool LoadModel()
        {
            bool ret = false;
            appModel = ModelClassFactory.ActiveDoc;
            if (appModel != null) ret = true;
            return ret;
        }

        /// <summary>
        /// Загрузить неуправляемую модель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool LoadModel(ModelDoc2 model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Открыть модель по ссылке
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadModel(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Задать текущую модель как основную
        /// </summary>
        /// <returns></returns>
        public bool SetCurrentModelAsMain()
        {
            SwAppControl.MainModel = appModel.SwModel;
            return appModel != null ? true : false;
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
