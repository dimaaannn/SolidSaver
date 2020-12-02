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


}