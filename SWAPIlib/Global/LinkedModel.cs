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
using System.IO;

namespace SWAPIlib.Global
{

    /// <summary>
    /// Обёртка для управления моделями и их загрузки
    /// </summary>
    public interface ILinkedModel 
    {
        /// <summary>
        /// Коренная модель
        /// </summary>
        IAppModel AppModel { get;}
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
        /// Дерево компонентов
        /// </summary>
        AssemblyTree SubComponents { get; }
        ObservableCollection<IComponentControl> SelectedComponents { get; }

        /// <summary>
        /// Загрузить список дочерних компонентов
        /// </summary>
        /// <returns></returns>
        bool GetSubComponents(Func<IComponentControl, bool> compFilter = null);
        bool? IsHaveSubComponents { get; }

        /// <summary>
        /// Модель была выгружена
        /// </summary>
        event EventHandler<SwEventArgs> ModelDisposed;

    }


    public class LinkedModel : ILinkedModel
    {
        public LinkedModel(IAppModel model)
        {
            _AppModel = model;
            _selectedComponents = new ObservableCollection<IComponentControl>();
        }

        private IAppModel _AppModel;
        private AssemblyTree _subComponents;

        public IAppModel AppModel => _AppModel;

        #region PropProxy
        public AppDocType DocType => AppModel is null ? AppDocType.swNONE : AppModel.DocType;
        /// <summary>
        /// Является ли модель сборкой
        /// </summary>
        private bool IsAssembly => DocType == AppDocType.swASM;
        public string Title => AppModel?.Title; //TODO add override for types
        public string Path => AppModel?.Path;
        public IList<IPropertyModel> ActivePropList { get => AppModel?.PropList; }

        public string ActiveConfigName //TODO добавить класс детали
        {
            get
            {
                if (AppModel is AppAssembly assembly)
                    return assembly.ConfigName;
                else return "Отсутствует";
            }
            set
            {
                if (AppModel is AppAssembly assembly)
                    assembly.ConfigName = value;
            }
        }

        #endregion

        /// <summary>
        /// Дерево компонентов
        /// </summary>
        public AssemblyTree SubComponents
        { get => _subComponents; }
        /// <summary>
        /// Список прикреплённых свойств
        /// </summary>
        public ISwProperty MainInfoProp { get; set; }
        public IFileModelProp ProjectNameProp { get; set; }

        private ObservableCollection<IComponentControl> _selectedComponents;
        public ObservableCollection<IComponentControl> SelectedComponents { get => _selectedComponents; set => _selectedComponents = value; }

        private bool? _IsHaveSubComponents;

        public bool? IsHaveSubComponents => _IsHaveSubComponents;

        public event EventHandler<SwEventArgs> ModelDisposed;

        public bool GetSubComponents(Func<IComponentControl, bool> compFilter = null)
        {
            bool ret = false;
            if (AppModel is IAppAssembly swAssembly)
            {
                _subComponents = new AssemblyTree(swAssembly, compFilter);
                if (SubComponents.SubComponents.Count > 0)
                    ret = true;
            }
            _IsHaveSubComponents = ret;
            return ret;
        }

    }

}
