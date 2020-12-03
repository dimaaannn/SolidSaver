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
            _tempPropertyValue = null;
            _propertyValue = null;
            if (IsValid && IsReadable)
            {
                _propertyValue = ReadValue();
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
        public override PropValidator Validator { get => PropValidatorTemplate.IsPartOrAsm; }

        public override string ReadValue() => ModelConfigProxy.GetConfParam(
                    AppModel.SwModel, ConfigName, PropertyName);

    }

    public class FieldProperty : AppPropertyBase
    {
        public FieldProperty(AppModel appModel)
        {
            _appModel = appModel;
            IsReadable = true;
        }

        public override PropValidator Validator => PropValidatorTemplate.IsExist;
        public override string ReadValue() => _appModel.FileName;
        public override void Update()
        {
            PropertyValue = ReadValue();
        }

        protected override bool CheckWrite()
        {
            throw new NotImplementedException();
        }
        public override bool WriteValue()
        {
            throw new NotImplementedException();
        }

    }
}