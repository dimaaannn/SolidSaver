using System.Linq;
using SWAPIlib.BaseTypes;
using SWAPIlib.MProperty.BaseProp;

namespace SWAPIlib.MProperty
{
    /// <summary>
    /// Именованное свойство модели SW
    /// </summary>
    public class PropBindNamed : PropBindSWModel<IAppModel>
    {
        protected PropBindNamed() :base() { } 
        /// <summary>
        /// Привязка к активной конфигурации модели
        /// </summary>
        /// <param name="target">Модель привязки</param>
        /// <param name="propName">Имя свойства</param>
        public PropBindNamed(IAppModel target, string propName) :base(target)
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
            IAppModel target, string propName, string configName)
            : this(target, propName)
        {
            ConfigName = configName;
        }

        /// <summary>
        /// Имя свойства
        /// </summary>
        public string PropertyName
        {
            //Свойство по умолчанию
            get => propertyName;
            set
            {
                CheckPropValidation(value);

                propertyName = value;
                RaiseTargetChanged(TargetRef);
            }
        }
        private string propertyName;

        public override string Title => $"{PropertyName}:";

        /// <summary>
        /// Получить значение свойства
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetValue(IAppModel model, string propName, string configName)
        {
            return model[configName, propName];
        }

        /// <summary>
        /// Свойство активной конфигурации
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            string ret;
            ret = GetValue(TargetRef, PropertyName, ConfigName);
            if (!IsCurrentPropertyValid && string.IsNullOrEmpty(ret))
                ret = @"$NOT FOUND$";

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
            return SetValue(TargetRef, PropertyName, ConfigName, newValue: newValue);
        }
        /// <summary>
        /// Проверить наличие конкретного свойства в модели
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        public override bool Validator(IAppModel targetRef)
        {
            bool ret = true;

            if (targetRef is AppModel)
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
        protected bool CheckPropValidation(string propName)
        {
            return IsCurrentPropertyValid = IsPropertyValid(TargetRef, propName);
        }
        bool IsCurrentPropertyValid { get; set; }

        public override object Clone()
        {
            var newProp = new PropBindNamed()
            {
                TargetRef = this.targetRef,
                ConfigName = this.configName,
                PropertyName = this.propertyName,

                IsReadable = this.IsReadable,
                IsWritable = this.IsWritable
            };
            return newProp;
        }

        public override string BindingInfo => bindingInfo;
        private static readonly string bindingInfo = 
            "Именованное свойство модели задаваемое пользователем";
    }
}
