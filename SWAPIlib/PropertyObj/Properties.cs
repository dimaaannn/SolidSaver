using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace SWAPIlib
{
    /// <summary>
    /// Возможность доступа к свойству
    /// </summary>
    /// <param name="appModel"></param>
    /// <returns></returns>
    public delegate bool PropValidator(AppModel appModel);

    /// <summary>
    /// Конвертер свойств в нужный формат
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="outvalue"></param>
    /// <returns></returns>
    public delegate bool PropValueConvertor<T>(T value, out T outvalue);

    public static class PropValidatorTemplate
    {

        /// <summary>
        /// Шаблон - является деталью или сборкой
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsPartOrAsm(AppModel appModel)
        {
            AppDocType type = appModel.DocType;
            if (type == AppDocType.swASM || type == AppDocType.swPART)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Шаблон - является деталью или сборкой или компонентом
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsPartOrAsmOrComp(AppModel appModel)
        {
            AppDocType type = appModel.DocType;
            if (type == AppDocType.swASM ||
                type == AppDocType.swPART ||
                type == AppDocType.swCOMPONENT)
                return true;
            else
                return false;
        }

        public static bool StringPropertyConvert(string value, out string outvalue)
        {
            bool ret = false;
            if (!String.IsNullOrEmpty(value))
            {
                ret = true;
                outvalue = value;
            }
            else
                outvalue = null;

            return ret;
        }
    }


    public abstract class AppModelProp : ISwProperty
    {
        public AppModelProp(AppModel appModel)
        {
            this.AppModel = appModel;
        }

        public AppModelProp() { }

        public AppModel AppModel
        {
            get => _appModel;
            set
            {
                _appModel = value;
                IsValid = (bool)Validator?.Invoke (_appModel);
            }
        }

        public virtual bool IsReadable { get; set; }

        public virtual bool IsWritable { get; set; }

        public virtual string UserName { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual bool IsValid { get; private set; }
        public virtual string PropertyValue
        {
            get => _propertyValue;
            set => _tempPropertyValue = value;
        }

        public virtual string ConfigName
        {
            get => _configName ?? ModelConfigProxy.GetActiveConfName(AppModel.SwModel);
            set => _configName = value;
        }

        public PropValidator Validator { get; set; }

        protected AppModel _appModel;
        protected string _propertyValue;
        protected string _tempPropertyValue;
        protected string _configName = null;

        public abstract void Update();

        public abstract bool WriteValue();

        /// <summary>
        /// Проверка перед записью значения
        /// </summary>
        /// <returns></returns>
        protected abstract bool CheckWrite();
    }

    /// <summary>
    /// свойство модели
    /// </summary>
    public class AppBaseModelProp : AppModelProp
    {
        public AppBaseModelProp(AppModel appModel) : base(appModel)
        {
            Validator = PropValidatorTemplate.IsPartOrAsmOrComp;
        }


        public override void Update()
        {
            _tempPropertyValue = null;
            _propertyValue = null;
            if (IsValid && IsReadable)
            {
                _propertyValue = ModelConfigProxy.GetConfParam(
                    AppModel.SwModel, ConfigName, PropertyName);
            }
        }

        public override bool WriteValue()
        {
            bool ret = false;
            if (CheckWrite())
            {
                ret = ModelConfigProxy.SetConfParam(AppModel.SwModel,
                    ConfigName, PropertyName, _tempPropertyValue);
            }
            return ret;
        }
        /// <summary>
        /// Проверка перед записью значения
        /// </summary>
        /// <returns></returns>
        protected override bool CheckWrite()
        {
            bool ret = false;
            if (
                IsWritable &&
                !String.IsNullOrEmpty(_tempPropertyValue) &&
                _tempPropertyValue != PropertyValue)
            {
                ret = true;
            }

            return ret;
        }
    }

    public class FileModelProp : IFileModelProp
    {
        public bool IsRoot { get; set; }
        public string WorkFolder { get; set; }
        public string ModelFolder { get => Path.GetDirectoryName( _appModel.Path); }
        public string GetProjectData 
        {
            get
            {
                string key = "ProjectData";
                if (DataDict.ContainsKey(key))
                    return DataDict[key];
                else
                {
                    var match = ReProjectData.Match(_appModel.Path);
                    return DataDict[key] = match?.Groups[1].Value;
                }
            }
        }  
        public string ProjectNumber { get; }
        public string ProjectClient { get; }
        public string ProjectName { get; }
        public string ProjectType { get; }

        public bool IsImported { get; }
        public bool IsLibModel { get; }

        public bool IsSheetPart { get; }
        public bool IsHasDrawing { get; }

        public FileModelProp(AppModel appmodel)
        {
            _appModel = appmodel;
            DataDict = new Dictionary<string, string>();
        }

        private AppModel _appModel;
        private readonly Dictionary<string, string> DataDict;
        private static Regex ReProjectData = new Regex(
            @"\\(.?\d+ ?-[^\\]*)");

    }

}
