using System.Linq;
using SWAPIlib.BaseTypes;
using SWAPIlib.MProperty.BaseProp;

namespace SWAPIlib.MProperty
{
    /// <summary>
    /// Именованное свойство модели SW
    /// </summary>
    public class PropBindNamed : PropBindSWModel<IModelEntity>
    {
        public PropBindNamed() :base() { } 
        /// <summary>
        /// Привязка к активной конфигурации модели
        /// </summary>
        /// <param name="target">Модель привязки</param>
        /// <param name="propName">Имя свойства</param>
        public PropBindNamed(IModelEntity target, string propName) :base(target)
        {
            IsReadable = true;
            IsWritable = true;
            PropertyName = propName;
        }
        /// <summary>
        /// Привязка к свойству в конфигурации
        /// </summary>
        /// <param name="target">Цель привязки</param>
        /// <param name="propName">Имя свойства</param>
        /// <param name="configName">Имя конфигурации</param>
        public PropBindNamed(
            IModelEntity target, string propName, string configName)
            : this(target, propName)
        {
            ConfigName = configName;
        }

        /// <summary>
        /// Имя свойства
        /// </summary>
        public string PropertyName { get => propertyName; set => propertyName = value; }
        private string propertyName;

        public override string DisplayPropName => $"{PropertyName}:";

        /// <summary>
        /// Получить значение свойства
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetValue(IAppModel model, string propName, string configName)
        {
            return model?[configName, propName] ?? "$NOT FOUND$";
        }

        /// <summary>
        /// Свойство активной конфигурации
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            string ret;
            ret = GetValue(TargetWrapper?.TargetWrapper, PropertyName, ConfigName);
            if (!IsCurrentPropertyValid && string.IsNullOrEmpty(ret))
                ret = @"СВОЙСТВО_НЕ_НАЙДЕНО";

            return ret;
        }
        
        /// <summary>
        /// Статическая запись свойства
        /// </summary>
        /// <param name="model">Цель</param>
        /// <param name="propName">Имя параметра</param>
        /// <param name="configName">Имя конфигурации</param>
        /// <param name="newValue">Значение для записи</param>
        /// <returns></returns>
        public static bool SetValue(IAppModel model, string propName, string configName, string newValue)
        {
            return model.SetParameterVal(configName: configName, paramName: propName, newValue: newValue);
        }
        /// <summary>
        /// Записать значение свойства
        /// </summary>
        /// <param name="newValue">Значение</param>
        /// <returns></returns>
        public override bool SetValue(string newValue)
        {
            return SetValue(TargetWrapper.TargetWrapper, PropertyName, ConfigName, newValue: newValue);
        }
        /// <summary>
        /// Проверить наличие конкретного свойства в модели
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        public override bool Validator(IModelEntity targetRef)
        {
            bool ret = false;
            if(targetRef?.TargetWrapper?.ParameterList?.Contains(PropertyName) == true)
                ret = true;

            return ret;
        }
        /// <summary>
        /// Свойство пристутствует в модели
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static bool IsPropertyValid (IAppModel target, string propName)
        {
            var ret = false;
            if(!string.IsNullOrEmpty(propName))
                ret = target?.ParameterList.Contains(propName) ?? false;

            return ret;
        }

        /// <summary>
        /// Выдавать ли ошибку при запросе свойства
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected bool CheckPropValidation(string propName)
        {
            return IsCurrentPropertyValid = IsPropertyValid(TargetWrapper?.TargetWrapper, propName);
        }
        bool IsCurrentPropertyValid { get; set; }

        public override object Clone()
        {
            var newProp = new PropBindNamed()
            {
                TargetWrapper = this.targetWrapper,
                PropertyName = this.propertyName,
                
                IsReadable = this.IsReadable,
                IsWritable = this.IsWritable,
                
            };
            return newProp;
        }

        public override string BindingInfo => bindingInfo;
        private static readonly string bindingInfo = 
            "Именованное свойство модели задаваемое пользователем";
    }
}
