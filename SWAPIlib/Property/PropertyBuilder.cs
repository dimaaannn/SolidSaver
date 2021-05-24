using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property
{
    public interface IPropertyBuilder
    {
        /// <summary>
        /// Объект цели
        /// </summary>
        ITarget Target { get; set; }
        /// <summary>
        /// Обработчик
        /// </summary>
        IPropertyGetter2 Getter { get; set; }
        /// <summary>
        /// Настройки обработчика
        /// </summary>
        IPropertySettings Settings { get; set; }

        /// <summary>
        /// Свойство корректно и может быть создано
        /// </summary>
        bool IsPropertyValid { get; }
        /// <summary>
        /// Недостающие настройки обработчика
        /// </summary>
        HashSet<string> GetterRequirement { get; }

        IProperty Build();
        PropertySet CreateSettings(IPropertySettings propSetReference = null);
    }

    /// <summary>
    /// Создатель объектов свойств
    /// </summary>
    public class PropertyBuilder2 : IPropertyBuilder
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly DefaultPropertyBuilder DefaultBuilder = new DefaultPropertyBuilder();

        private IPropertyGetter2 getter;
        private IPropertySettings settings;

        public ITarget Target { get; set; }
        public IPropertyGetter2 Getter
        {
            get => getter;
            set
            {
                getter = value;
            }
        }
        public IPropertySettings Settings { get => settings; set => settings = value; }

        public bool IsPropertyValid => CheckPropertyValidation();
        public HashSet<string> GetterRequirement => CheckGetterRequirements();

        /// <summary>
        /// Создать объект свойства
        /// </summary>
        /// <returns></returns>
        public IProperty Build()
        {
            var ret = new Property.ComplexProperty(Target, Getter, Settings);
            //Add property to archive

            Logger.Debug("Property {name} created with target {TartetName}", ret.Name, Target.TargetName);
            return ret;
        }

        /// <summary>
        /// Обновить список недостающих параметров для обработчика
        /// </summary>
        /// <returns>Требования отсутствуют</returns>
        public HashSet<string> CheckGetterRequirements()
        {
            Logger.Trace("CheckGetterRequirements start");
            var requiredKeys = new HashSet<string>();
            requiredKeys.UnionWith(Getter.OptionsRequirement);

            if (Settings != null)
            {
                var settingsKeys = new HashSet<string>(Settings.Select(x => x.Key));
                requiredKeys.ExceptWith(settingsKeys);
            }
            return requiredKeys;
        }
        /// <summary>
        /// Проверить совместимость классов
        /// </summary>
        /// <returns></returns>
        protected bool CheckPropertyValidation()
        {
            Logger.Trace("CheckPropertyValidation start");
            if (
                Target != null
                && Getter != null
                && GetterRequirement.Count == 0
                )
                return true;
            else return false;
        }

        /// <summary>
        /// Создать дефолтные свойства, объединить объекты
        /// </summary>
        /// <param name="propSetReference"></param>
        /// <returns></returns>
        public PropertySet CreateSettings(IPropertySettings propSetReference = null)
        {
            PropertySet propSet;
            if (propSetReference is PropertySet set)
                propSet = set;
            else
                propSet = new PropertySet(Target);

            propSet.UnionPreserve(Settings);
            var defaultSettings = DefaultBuilder.FillDefaultProperty(this, propSet);
            propSet.UnionPreserve(defaultSettings);
            return propSet;
        }

        public override string ToString()
        {
            return Getter.Name;
        }
    }

    /// <summary>
    /// Класс создающий временный объект Builder для простых текстовых значений.
    /// </summary>
    class TextPropertyBuilder : IPropertyBuilder
    {

        public TextPropertyBuilder(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public ITarget Target { get => null; set { } }
        public IPropertyGetter2 Getter { get => null; set { } }
        public IPropertySettings Settings { get => null; set { } }

        public bool IsPropertyValid => !string.IsNullOrEmpty(Name) && Value != null;

        public HashSet<string> GetterRequirement => new HashSet<string>();

        public IProperty Build()
        {
            return new TextProperty(Name, Value);
        }
        public PropertySet CreateSettings(IPropertySettings propSetReference = null) => null;
    }



    /// <summary>
    /// Совместить в один
    /// </summary>
    public class PropertyBuilder
    {

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Обёртка для цели обработчика
        /// </summary>
        public ITarget Target;
        /// <summary>
        /// Непосредственный обработчик
        /// </summary>
        public IPropertyGetter2 PropertyGetter;
        private IPropertySettings propertySettings;

        /// <summary>
        /// Настройки обработчика
        /// </summary>
        public IPropertySettings PropertySettings { get => propertySettings;
            set
            {
                propertySettings = value;
                CheckRequirements();
            }
        }

        /// <summary>
        /// Список недостающих настроек после проверки
        /// </summary>
        public HashSet<string> SettingsRequirement { get; private set; }

        /// <summary>
        /// Проверка объектов на совместимость
        /// </summary>
        /// <returns></returns>
        public bool IsPropertyValid()
        {
            bool ret = false;

            if (Target != null && PropertyGetter != null)
            {
                ret = true;
                //Settings check
                //SettingsRequirement = GetterRequirements(PropertyGetter, PropertySettings);
                //IsSettingsValid = SettingsRequirement.Count > 0;

                //IsTargetValid = PropertyGetter.CheckTarget(Target, PropertySettings);

            }

            //ret &= IsSettingsValid;
            //ret &= IsTargetValid;

            return ret;
        }

        #region Объект цели

        /// <summary>
        /// Set target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public PropertyBuilder SetTarget(ITarget target)
        {
            Target = target;
            return this;
        }
        /// <summary>
        /// Create and set target
        /// </summary>
        /// <typeparam name="T">typeof target</typeparam>
        /// <param name="target"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public PropertyBuilder SetTarget<T>(T target, string targetName, TargetType targetType)
        {
            Target = new Target<T>(target, targetName, targetType);
            return this;
        }
        #endregion


        #region Обработчик

        /// <summary>
        /// Set getter
        /// </summary>
        /// <param name="getter"></param>
        /// <returns></returns>
        public PropertyBuilder SetGetter(IPropertyGetter2 getter)
        {
            PropertyGetter = getter;
            CheckRequirements();
            return this;
        }

        /// <summary>
        /// Список недостающих параметров для обработчика
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="propSettings"></param>
        /// <returns></returns>
        private HashSet<string> GetterRequirements(IPropertyGetter2 getter, IPropertySettings propSettings)
        {
            var requiredKeys = new HashSet<string>();
            requiredKeys.UnionWith(getter.OptionsRequirement);

            if (propSettings != null)
            {
                var settingsKeys = new HashSet<string>(propSettings.Select(x => x.Key));
                requiredKeys.ExceptWith(settingsKeys);
            }

            return requiredKeys;
        }

        private void CheckRequirements() => SettingsRequirement = GetterRequirements(PropertyGetter, PropertySettings);
        #endregion


        #region Настройки обработчика

        /// <summary>
        /// Задать объект настроек
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public PropertyBuilder SetPropertySettings(IPropertySettings settings)
        {
            PropertySettings = settings;
            return this;
        }
        /// <summary>
        /// Создать объект свойств
        /// </summary>
        /// <returns></returns>
        public IPropertySet CreateSettings(ITarget target)
        {
            return new PropertySet(target);
        }
        #endregion

        public IProperty Build()
        {
            var ret = new Property.ComplexProperty(Target, PropertyGetter, PropertySettings);
            Logger.Debug("Property {name} created with target {TartetName}", ret.Name, Target.TargetName);
            return ret;
        }
    }

}
