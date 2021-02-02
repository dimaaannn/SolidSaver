using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SwConst;
using System.Diagnostics;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.MProperty;

namespace SWAPIlib.BaseTypes
{

    /// <summary>
    /// Базовый класс модели
    /// </summary>
    public class AppModel : IAppModel
    {
        /// <summary>
        /// ModelDoc2 Constructor
        /// </summary>
        /// <param name="swModel"></param>
        public AppModel(ModelDoc2 swModel)
        {
            PropList = new List<IPropertyModel>();
            IsExist = true;
            DocType = PartTypeChecker.GetSWType(swModel);

            if (swModel != null)
            {
                //TODO Add default properties to list
                _swModel = swModel;
                Path = ModelProxy.GetPathName(SwModel);
                ModelEntity = new ModelEntity(this);
                PropList = PropModelFactory.DefaultModel(ModelEntity);

                #region EventProxy
                if (DocType == AppDocType.swASM)
                {
                    (swModel as AssemblyDoc).DestroyNotify += CloseFileHandler;
                }
                else if (DocType == AppDocType.swPART)
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

        private ModelDoc2 _swModel;
        public bool IsExist { get; private set; } //Удалить
        private List<string> _configList;


        /// <summary>
        /// Список базовых свойств модели
        /// </summary>
        public List<IPropertyModel> PropList { get; protected set; } //Todo Заменить на актуальный
        

        /// <summary>
        /// Событие - файл был закрыт
        /// </summary>
        public event EventHandler<SwEventArgs> FileIsClosed;
        /// <summary>
        /// Прокси для события закрытия файла
        /// </summary>
        /// <returns></returns>
        private int CloseFileHandler()
        {
            IsExist = false;
            string evT = $"Document {FileName} closed";
            var evArg = new SwEventArgs(evT);
            FileIsClosed?.Invoke(this, evArg);
            return 0;
        }

        /// <summary>
        /// Сравнить по типу документа
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            if (other is IAppModel model)
                return this.DocType.CompareTo(model.DocType);
            else return 0;
        }

        /// <summary>
        /// Ссылка на объект модели SolidWorks
        /// </summary>
        public ModelDoc2 SwModel { get => _swModel; }
        /// <summary>
        /// Тип модели
        /// </summary>
        public virtual AppDocType DocType { get; }
        /// <summary>
        /// Имя модели
        /// </summary>
        public virtual string Title { get => ModelProxy.GetName(_swModel); }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// Имя файла модели
        /// </summary>
        public string FileName { get => System.IO.Path.GetFileName(Path); }

        /// <summary>
        /// Список имён конфигураций
        /// </summary>
        public virtual List<string> ConfigList
        {
            get
            {
                if (_configList == null)
                    _configList = new List<string>(SwModel.GetConfigurationNames());
                return _configList;
            }
        }
        /// <summary>
        /// Статус отображения
        /// </summary>
        public virtual bool VisibState
        {
            get => SwModel.Visible;
            set => SwModel.Visible = value;
        }
        /// <summary>
        /// Получить или задать имя активной конфигурации
        /// </summary>
        public string ActiveConfigName 
        { 
            get => ModelConfigProxy.GetActiveConfName(SwModel);
            set => ModelConfigProxy.SetActiveConf(SwModel, value);
        }
        /// <summary>
        /// Задать активную конфигурацию с проверкой
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        bool IAppModel.SetActiveConfig(string configName)
        {
            return ModelConfigProxy.SetActiveConf(SwModel, configName);
        }

        /// <summary>
        /// Список свойств модели
        /// </summary>
        public string[] ParameterList => SwModel.GetCustomInfoNames2(ActiveConfigName);
        /// <summary>
        /// Задать значение именованного свойства
        /// </summary>
        /// <param name="configName">Имя конфигурации</param>
        /// <param name="paramName">Имя свойства</param>
        /// <param name="newValue">Значение свойства</param>
        /// <returns>Успешная запись</returns>
        public bool SetParameterVal(string configName, string paramName, string newValue)
        {
            return ModelConfigProxy.SetConfParam(SwModel, configName, paramName, newValue);
        }
        /// <summary>
        /// Получить свойство в конфигурации
        /// </summary>
        /// <param name="configName">имя конфигурации</param>
        /// <param name="paramName">имя свойства</param>
        /// <returns></returns>
        public string this[string configName, string paramName]
        {
            get => ModelConfigProxy.GetConfParamValue(SwModel, configName, paramName);
            set => ModelConfigProxy.SetConfParam(SwModel, configName, paramName, value);
        }
        /// <summary>
        /// Получить свойство в активной конфигурации
        /// </summary>
        /// <param name="paramName">имя свойства</param>
        /// <returns></returns>
        public string this[string paramName]
        {
            get => ModelConfigProxy.GetConfParamValue(SwModel, ActiveConfigName, paramName);
            set => ModelConfigProxy.SetConfParam(SwModel, paramName, value);
        }

        /// <summary>
        /// Объект сущности
        /// </summary>
        public IModelEntity ModelEntity { get; private set; }
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
