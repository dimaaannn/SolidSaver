using System;
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
        }

        public override void Update()
        {
            if (IsValid && IsReadable)
            {
                _propertyValue = ReadValue();
                if (_tempPropertyValue == null)
                    _tempPropertyValue = _propertyValue;
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
                !String.IsNullOrEmpty(_tempPropertyValue) &&
                _tempPropertyValue != PropertyValue)
            {
                ret = true;
            }

            return ret;
        }
        private PropValidator _validator = PropValidatorTemplate.IsPartOrAsmOrComp;
        public override PropValidator Validator { get => _validator; set => _validator = value; }

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
        }
        private PropValidator _validator = PropValidatorTemplate.IsExist;
        public override PropValidator Validator
        {
            get => _validator;
            set => _validator = value;
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
        public static class ModelProp
        {
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

    public static class PropSheetTemplate
    {
        public static IList<ISwProperty> Component(SwComponent swComponent)
        {
            var ret = new List<ISwProperty>();
            ret.Add(PropertyFactory.ModelProp.Nomination(swComponent));
            ret.Add(PropertyFactory.ModelProp.Denomination(swComponent));
            return ret;
        }
    }
}