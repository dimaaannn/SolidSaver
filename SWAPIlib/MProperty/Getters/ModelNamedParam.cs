using System.Diagnostics;
using System.Linq;
using SWAPIlib.BaseTypes;

namespace SWAPIlib.MProperty.Getters
{
    /// <summary>
    /// Именованное свойство модели SW
    /// </summary>
    public class PropModelNamedParamGetter : PropModelGetterBase<IModelBinder>
    {
        public PropModelNamedParamGetter() : base()
        {
            IsReadable = true;
            IsWritable = true;
        }

        private string propertyName;
        private static readonly string bindingInfo =
            "Именованное свойство модели задаваемое пользователем";

        /// <summary>
        /// Имя свойства
        /// </summary>
        public string PropertyName { get => propertyName; set => propertyName = value; }
        /// <summary>
        /// Имя свойства для пользователя
        /// </summary>
        public override string DisplayPropName => $"{PropertyName}:";

        /// <summary>
        /// Получить значение свойства
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public override string GetValue(IModelBinder binder)
        {
            Debug.Write($"Get NamedProperty in {binder.TargetName} - {binder.ConfigName}: {PropertyName}");
            string ret = binder.Target?.TargetWrapper
                [binder.ConfigName, PropertyName]
                ?? "Имя свойства не найдено";

            Debug.WriteLine($"Value = {ret}");
            return ret;
        }
        /// <summary>
        /// Задать значение свойства
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public override bool SetValue(IModelBinder binder, string newValue)
        {
            bool ret = false;
            Debug.WriteLine($"SetNamedProperty: {PropertyName}: {newValue} to {binder.Target.Title} - {binder.ConfigName}");
            ret = binder.Target.TargetWrapper.SetParameterVal(
                binder.ConfigName, PropertyName, newValue
                );
            Debug.WriteLine(ret ? "Success" : "Failed");

            return ret;
        }
        /// <summary>
        /// Проверить наличие конкретного свойства в модели
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        public override bool Validator(IModelBinder binder)
        {
            bool ret = false;
            if (binder?.Target?.TargetWrapper is IAppModel model)
                ret = model.ParameterList.Contains(PropertyName);

            return ret;
        }
        public object Clone()
        {
            var newProp = new PropModelNamedParamGetter()
            {
                PropertyName = propertyName,
                IsReadable = IsReadable,
                IsWritable = IsWritable,
            };
            return newProp;
        }
        public override string PropertyInfo => bindingInfo;
    }
}
