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
    public delegate bool PropValidator (ISwModel appModel);

    /// <summary>
    /// Конвертер свойств в нужный формат
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="outvalue"></param>
    /// <returns></returns>
    public delegate bool PropValueConvertor<T>(T value, out T outvalue);

    public delegate string PropValueGetter();

    public static class PropValidatorTemplate
    {
        /// <summary>
        /// Ненулевая ссылка на модель
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsExist(ISwModel appModel)
        {
            if (appModel != null) return true;
            else return false;
        }

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
        public static bool IsPartOrAsmOrComp(ISwModel appModel)
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


    /// <summary>
    /// Базовый класс свойств
    /// </summary>
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
    /// Базовый класс свойств для моделей
    /// </summary>
    public abstract class AppPropertyBase : ISwProperty
    {
        public virtual AppModel AppModel
        {
            get => _appModel;
            set
            {
                _appModel = value;
                IsValid = true;
            }
        }

        public virtual bool IsReadable { get; set; }

        public virtual bool IsWritable { get; set; }

        public virtual string UserName { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual bool IsValid { get; protected set; }
        public virtual string PropertyValue
        {
            get
            {

                if (_propertyValue != null)
                    return _tempPropertyValue ?? _propertyValue;
                else
                    return _propertyValue = ReadValue();
            }
            set => _tempPropertyValue = value;
        }

        public virtual bool IsModifyed { get => _tempPropertyValue != null; }

        public virtual string ConfigName
        {
            get
            {
                string ret = null;
                if (AppModel is ISwComponent swComp)
                    ret = _configName ?? swComp.ConfigName;

                if (AppModel is ISwPart swPart)
                    ret = _configName ?? swPart.ConfigName;

                if (AppModel is ISwAssembly swAsm)
                    ret = _configName ?? swAsm.ConfigName;

                return ret;
            }
            set => _configName = value;
        }

        public virtual PropValidator Validator { get; set; }
        public abstract string ReadValue();

        protected AppModel _appModel;
        protected string _propertyValue;
        protected string _tempPropertyValue = null;
        protected string _configName = null;

        public virtual void Update()
        {
            if (IsReadable)
            {
                if (Validator(_appModel))
                {
                    _propertyValue = ReadValue();
                }
                _tempPropertyValue = null;
            }
        }

        public abstract bool WriteValue();

    }

    public abstract class ComponentProperty : AppPropertyBase
    {
        public IAppComponent AppComponent { get; set; }
        public override AppModel AppModel 
        { 
            get => AppComponent.PartModel;
            set
            {
                _appModel = value;
                IsValid = true;
            }
        }
        public override string ConfigName
        {
            get => _configName ?? AppComponent.RefConfigName;
            set => _configName = value;
        }
    }

    /// <summary>
    /// Generic abstract property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AppProperty<T> : AppPropertyBase, ISwProperty<T>
    {
        public T RawPropertyValue { get; set; }
        public abstract T ReadRawValue();
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
