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
        private ModelDoc2 _swModel;

        public bool IsExist { get; private set; }
        public ModelDoc2 SwModel { get => _swModel; }
        public virtual AppDocType DocType { get; }
        public string FileName { get => System.IO.Path.GetFileName(Path); }
        public virtual string Title { get => ModelProxy.GetName(_swModel); }
        public string Path { get; }
        public List<ISwProperty> PropList { get; protected set; }
        public IFileModelProp GlobalModelProp { get; } //TODO remove

        public event EventHandler<SwEventArgs> FileIsClosed;

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
                _swModel = swModel;
                Path = ModelProxy.GetPathName(SwModel);


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

                ModelEntity = new ModelEntity(this);
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
            FileIsClosed?.Invoke(this, evArg);
            return 0;
        }

        public int CompareTo(object other)
        {
            if (other is IAppModel model)
                return this.DocType.CompareTo(model.DocType);
            else return 0;
        }

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
        private List<string> _configList;

        public virtual bool VisibState
        {
            get => SwModel.Visible;
            set => SwModel.Visible = value;
        }

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

        public string[] ParameterList => SwModel.GetCustomInfoNames2(ActiveConfigName);
        /// <summary>
        /// Объект сущности
        /// </summary>
        public IModelEntity ModelEntity { get; private set; }

        public string this[string configName, string paramName]
        {
            get => ModelConfigProxy.GetConfParamValue(SwModel, configName, paramName);
            set => ModelConfigProxy.SetConfParam(SwModel, configName, paramName, value);
        }
        public string this[string paramName]
        {
            get => ModelConfigProxy.GetConfParamValue(SwModel, ActiveConfigName, paramName);
            set => ModelConfigProxy.SetConfParam(SwModel, paramName, value);
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
