using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.ComConn;
using SWAPIlib.MProperty;

namespace SWAPIlib
{
    public class AppComponent : IAppComponent
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="swComp2"></param>
        public AppComponent(Component2 swComp2)
        {
            if(swComp2 != null)
            {
                PropList = new List<IPropertyModel>();


                //PropList.AddRange(PropertyObj.CompPropertyFactory. //TODO Создать список свойств для компонента
                //    ComponentMainProp(this));

                _swCompModel = swComp2;
                DocType = AppDocType.swNONE;

                var swModel = ComponentProxy.GetModelDoc2(swComp2);

                //Если компонент не погашен
                if (swModel != null)
                {
                    _appModel = ModelClassFactory.GetModel(swModel);
                    IsExist = true;
                    DocType = _appModel.DocType;
                    PropList.AddRange(_appModel.PropList);
                }
            }
        }

        private AppModel _appModel = null;
        private Component2 _swCompModel;

        /// <summary>
        /// Объект сопряжённой модели
        /// </summary>
        public AppModel PartModel => _appModel;
        /// <summary>
        /// Объект модели SW
        /// </summary>
        public ModelDoc2 SwModel => PartModel?.SwModel;
        /// <summary>
        /// Тип документа
        /// </summary>
        public virtual AppDocType DocType { get; private set; }
        /// <summary>
        /// Объект компонента SW
        /// </summary>
        public Component2 SwCompModel => _swCompModel;


        public virtual bool IsExist { get; private set; } //Удалить
        public List<IPropertyModel> PropList { get; private set; } //Переделать

        /// <summary>
        /// Получить сущность
        /// </summary>
        public IModelEntity ModelEntity => PartModel?.ModelEntity; //TODO переделать на собственную сущность
        /// <summary>
        /// Путь к файлу привязанного к компоненту
        /// </summary>
        public string Path => ComponentProxy.GetPathName(SwCompModel);
        /// <summary>
        /// Имя файла
        /// </summary>
        public virtual string FileName => System.IO.Path.GetFileName(Path);
        /// <summary>
        /// Имя компонента
        /// </summary>
        public string Title => ComponentProxy.GetName(_swCompModel);

        /// <summary>
        /// Получить или задать имя зависимой конфигурации в сборке
        /// </summary>
        public string ActiveConfigName
        {
            get => ComponentProxy.GetRefConfig(SwCompModel);
            set => SwCompModel.ReferencedConfiguration = value;
        }
        /// <summary>
        /// Список конфигураций
        /// </summary>
        public List<string> ConfigList { get => PartModel?.ConfigList;} //Переделать на загрузку из компонента
        /// <summary>
        /// Статус компонента в сборке
        /// </summary>
        public AppSuppressionState SuppressionState =>
            PartTypeChecker.GetAppSuppressionState(SwCompModel);
        /// <summary>
        /// Получить или задать статус видимости
        /// </summary>
        public bool VisibState
        {
            get => (int)ComponentProxy.GetVisibleStatus(SwCompModel) == 1;
            set => ComponentProxy.SetVisibleStatus(SwCompModel, 
                value ? AppCompVisibility.Visible : AppCompVisibility.Hidden);
        }
        /// <summary>
        /// Исключить из спецификации
        /// </summary>
        public bool ExcludeFromBOM
        {
            get => SwCompModel.ExcludeFromBOM;
            set => SwCompModel.ExcludeFromBOM = value;
        }
        /// <summary>
        /// Событие закрытия файла
        /// </summary>
        public event EventHandler<SwEventArgs> FileIsClosed
        {
            add => throw new NotImplementedException();
            remove { }
        }
        /// <summary>
        /// цвет компонента (получить или задать)
        /// </summary>
        public MaterialProperty MaterialColor
        {
            get => ComponentProxy.GetMaterialProperty(SwCompModel);
            set => ComponentProxy.SetMaterialProperty(SwCompModel, value);
        }

        /// <summary>
        /// Получить именованное свойство
        /// </summary>
        string[] IAppModel.ParameterList => PartModel?.ParameterList; //TODO переделать на получение из компонента
        /// <summary>
        /// Получить или задать именованное свойство в конфигурации
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        string IAppModel.this[string configName, string paramName] //TODO переделать на получение свойств компонента
        {
            get => PartModel?[configName: configName, paramName: paramName];
            set
            {
                if(PartModel != null)
                    PartModel[configName: configName, paramName: paramName] = value;
            }
        }
        /// <summary>
        /// Получить или задать именованное свойство
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        string IAppModel.this[string paramName] 
        {
            get => PartModel?[paramName: paramName];
            set
            {
                if (PartModel != null)
                    PartModel[paramName: paramName] = value;
            }
        }

        /// <summary>
        /// Количество дочерних компонентов
        /// </summary>
        /// <returns></returns>
        public int GetChildrenCount() => SwCompModel.IGetChildrenCount();
        /// <summary>
        /// Получить список дочерних компонентов
        /// </summary>
        /// <param name="TopLeverOnly"></param>
        /// <returns></returns>
        public List<IAppComponent> GetComponents(bool TopLeverOnly)
        {
            var ret = new List<IAppComponent>();
            var components = ComponentProxy.GetChildren(SwCompModel);
            Debug.WriteLine($"GetComponents from {this.FileName} begin");
            foreach (Component2 comp in components)
            {
                ret.Add(new AppComponent(comp));
            }
            Debug.WriteLine(ret.Count > 0 ? "Success" : "No components");

            return ret;
        }
        /// <summary>
        /// Получить базовый компонент
        /// </summary>
        /// <returns></returns>
        public IAppComponent GetRootComponent()
        {
            return new AppComponent(ComponentProxy.GetRoot(SwCompModel));
        }
        /// <summary>
        /// Получить родительский компонент
        /// </summary>
        /// <returns></returns>
        public IAppComponent GetParent()
        {
            return new AppComponent(ComponentProxy.GetParent(SwCompModel));
        }

        /// <summary>
        /// Задать имя активной конфигурации
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        bool IAppModel.SetActiveConfig(string configName)
        {
            return ComponentProxy.SetRefConfig(SwCompModel, configName);
        }
        /// <summary>
        /// Прокси установки свойства модели
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="paramName"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        bool IAppModel.SetParameterVal(string configName, string paramName, string newValue)
        {
            bool ret = false;
            if (PartModel != null)
                ret = PartModel.SetParameterVal(configName: configName, paramName: paramName, newValue);
            return ret;
        }

        public int CompareTo(object obj)
        {
            if (obj is IAppModel model)
                return this.DocType.CompareTo(model.DocType);
            else return 0;
        }
        public bool Equals(AppComponent comp)
        {
            if (comp.FileName == this.FileName &&
                comp.ActiveConfigName == this.ActiveConfigName)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return $"{Title} - {ActiveConfigName}";
        }
        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;
            else
            {
                return this.Equals(obj as AppComponent);
            }
        }

    }
}
