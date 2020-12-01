using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SwConst;

namespace SWAPIlib
{
    
    public static class PartTypeChecker
    {

        /// <summary>
        /// Определить тип модели
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AppDocType GetSWType(ModelDoc2 model)
        {
            AppDocType ret = AppDocType.swNONE;
            swDocumentTypes_e swType;
            if (model is ModelDoc2 swModel)
            {
                swType = (swDocumentTypes_e)swModel.GetType();
                switch (swType)
                {
                    case swDocumentTypes_e.swDocASSEMBLY:
                        ret = AppDocType.swASM;
                        break;
                    case swDocumentTypes_e.swDocPART:
                        ret = AppDocType.swPART;
                        break;
                    case swDocumentTypes_e.swDocDRAWING:
                        ret = AppDocType.swDRAWING;
                        break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Является ли модель сборкой
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsAsm(AppModel appModel)
        {
            if (appModel.DocType == AppDocType.swASM)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Является ли модель деталью
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsPart(AppModel appModel)
        {
            if (appModel.DocType == AppDocType.swPART)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Содержит листовые тела
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsSheet(AppModel appModel)
        {
            return PartDocProxy.IsSheetMetal(appModel.SwModel);
        }

        /// <summary>
        /// Существование черетежа с тем же именем
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsHaveDrawing(AppModel appModel)
        {
            string partName = System.IO.Path.GetFileNameWithoutExtension(appModel.Path);
            string folder = System.IO.Path.GetDirectoryName(appModel.Path);

            string drawingExtension = ".SLDDRW";
            string DrawPath = $"{folder}\\{partName}{drawingExtension}";

            return System.IO.File.Exists(DrawPath);
        }


    }

    /// <summary>
    /// Возможность доступа к свойству
    /// </summary>
    /// <param name="appModel"></param>
    /// <returns></returns>
    public delegate bool PropValidator(AppModel appModel);
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




    /// <summary>
    /// свойство модели
    /// </summary>
    public class AppBaseModelProp  : ISwProperty
    {

        public AppModel AppModel 
        { 
            get => _appModel;
            set
            {
                _appModel = value;
                IsValid = Validator(_appModel);
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

        public PropValidator Validator = PropValidatorTemplate.IsPartOrAsmOrComp;

        private AppModel _appModel;
        private string _propertyValue;
        private string _tempPropertyValue;
        private string _configName = null;

        public virtual void Update()
        {
            _tempPropertyValue = null;
            _propertyValue = null;
            if (IsValid && IsReadable)
            {
                _propertyValue = ModelConfigProxy.GetConfParam(
                    AppModel.SwModel, ConfigName, PropertyName);
            }
        }

        public virtual bool WriteValue()
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
        private bool CheckWrite()
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
}
