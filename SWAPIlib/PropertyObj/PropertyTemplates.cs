﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{


    //var prop = new AppModelPropGetter(appmodel)
    //{
    //    IsReadable = true,
    //    IsWritable = true,
    //    PropertyName = "Наименование"
    //}
    //prop.Update();


    //TODO Переделать базовый
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

        public override void Update()
        {
            if (IsReadable)
            {
                if (Validator(_appModel))
                {
                    _propertyValue = ReadValue();
                }
                //_tempPropertyValue = null;
            }
        }

        public override bool IsValid => true; //TODO add is valid func to property
        public override bool WriteValue()
        {
            bool ret = false;
            if (CheckWrite())
            {
                ret = ModelConfigProxy.SetConfParam(AppModel.SwModel,
                    ConfigName, PropertyName, _tempPropertyValue);
                if (ret)
                    _tempPropertyValue = null;
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
                _tempPropertyValue != PropertyValue)
            {
                ret = true;
            }

            return ret;
        }

        public override string ReadValue()
        {
            if (AppModel.DocType == AppDocType.swPART || AppModel.DocType == AppDocType.swASM)
            {
                return ModelConfigProxy.GetConfParam(
                    AppModel.SwModel, ConfigName, PropertyName);
            }
            else
                return (AppModel as SwComponent).ConfigName;
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
        public override void Update()
        {
            PropertyValue = ReadValue();
        }

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
                AppModel appModel, string userName, string propertyName)
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

    //public static class PropSheetTemplate
    //{
    //    public static IList<ISwProperty> Component(AppComponent swComponent)
    //    {
    //        var ret = new List<ISwProperty>();
            
    //        return ret;
    //    }
    //}


    
}