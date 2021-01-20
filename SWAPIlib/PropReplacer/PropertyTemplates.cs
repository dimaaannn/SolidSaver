using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn.Proxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{


    /// <summary>
    /// Именованные свойства модели
    /// </summary>
    public class AppModelPropGetter : AppPropertyBase
    {
        public AppModelPropGetter(AppModel appModel) 
        {
            _appModel = appModel;
            Validator = PropValidatorTemplate.IsPartOrAsmOrComp;
        }


        public override bool IsValid => true; //TODO add is valid func to property
        public override bool WriteValue()
        {
            Debug.WriteLine($"Write val AppModelPropGetter {appModel.Title} - {_tempPropertyValue}");
            bool ret = false;
            if (CheckWrite())
            {
                ret = ModelConfigProxy.SetConfParam(appModel.SwModel,
                    ConfigName, PropertyName, _tempPropertyValue);
                if (ret)
                {
                    _propertyValue = _tempPropertyValue;
                    _tempPropertyValue = null;
                }
            }
            string WriteStatus = ret ? "written" : "Not written";
            Debug.Write($"Value was {WriteStatus}");
            //NotifyChanged
            RaisePropertyChanged("PropertyValue");
            RaisePropertyChanged("IsModifyed");
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
                _tempPropertyValue != _propertyValue && _tempPropertyValue != null)
            {
                ret = true;
            }

            return ret;
        }

        public override string ReadValue()
        {
            Debug.WriteLine($"ReadValue AppModelPropGetter {appModel.Title} - {UserName}");
            if (appModel.DocType == AppDocType.swPART || appModel.DocType == AppDocType.swASM)
            {
                return ModelConfigProxy.GetConfParam(
                    appModel.SwModel, ConfigName, PropertyName);
            }
            else
                throw new NotSupportedException("AppmodelPropGetter = not implemented type"); //TODO fix DrawingDoc type
        }

    }

    public class FieldProperty : AppPropertyBase
    {
        public FieldProperty(AppModel appModel)
        {
            _appModel = appModel;
            IsReadable = true;
            Validator = PropValidatorTemplate.IsExist;
        }

        public override string ReadValue() => _appModel.FileName;

        public override bool WriteValue()
        {
            throw new NotImplementedException();
        }

    }

    public static class PropertyFactory
    {
        public static class ModelProperty
        {
            public static List<ISwProperty> DefaultModelProp(AppModel app)
            {
                var ret = new List<ISwProperty>();
                ret.Add(Denomination(app));
                ret.Add(Nomination(app));
                return ret;
            }
            private static AppModelPropGetter CreateDefault(
                AppModel appModel, string userName, string propertyName, string confName = null)
            {
                return new AppModelPropGetter(appModel)
                {
                    IsReadable = true,
                    IsWritable = true,
                    Validator = PropValidatorTemplate.IsPartOrAsmOrComp,
                    UserName = userName,
                    PropertyName = propertyName
                };
            }
            public static AppModelPropGetter Denomination(AppModel appModel)
            {
                return CreateDefault(appModel, "Обозначение", "Обозначение");
            }
            public static AppModelPropGetter Nomination(AppModel appModel)
            {
                return CreateDefault(appModel, "Наименование", "Наименование");
            }

        }
    }




    
}