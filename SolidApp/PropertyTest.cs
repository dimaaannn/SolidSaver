using SolidWorks.Interop.sldworks;
using SWAPIlib.BaseTypes;
using SWAPIlib.Property;
using SWAPIlib.Property.ModelProperty;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidApp
{
    public  class PropertyTest
    {
        /// <summary>
        /// Агрегирующий класс
        /// </summary>
        public IProperty Property;
        /// <summary>
        /// Обёртка для цели обработчика
        /// </summary>
        public ITarget Target;

        /// <summary>
        /// Непосредственный обработчик
        /// </summary>
        public IPropertyGetter2 PropertyGetter;
        /// <summary>
        /// Настройки обработчика
        /// </summary>
        public IPropertySettings PropertySettings;
        /// <summary>
        /// Результат из обработчика
        /// Mожет выступать как настройка для других свойств
        /// </summary>
        public IComplexProperty Result;

        /// <summary>
        /// Сырой объект для привязки
        /// </summary>
        public ModelDoc2 SwModel;



        public PropertyTest()
        {


        }

        public void PrintResult()
        {

        }

        #region Target
        /// <summary>
        /// Установить цель
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(ITarget target) => Target = target;
        /// <summary>
        /// Создать цель на основе объекта
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ITarget<T> CreateTarget<T>(T target, string targetName, TargetType targetType, string Info = null)
        {
            var temp = new Target<T>(target, targetName, targetType) { TargetInfo = Info };
            return temp;
        }
        /// <summary>
        /// Создать обёртку
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public ITarget<ModelDoc2> CreateWrapperTarget(ModelDoc2 model)
        //{
        //    var wrapper = new SwModelWrapper(model);
        //    return wrapper;
        //}
        #endregion

        #region Getter
        /// <summary>
        /// Задать обработчик
        /// </summary>
        /// <param name="propertyGetter"></param>
        public void SetPropertyGetter(IPropertyGetter2 propertyGetter) => PropertyGetter = propertyGetter;
        /// <summary>
        /// Создать обработчик
        /// </summary>
        /// <returns></returns>
        public IPropertyGetter2 CreateActiveConfGetter() => new ActiveConfigNameGetter();
        public IPropertyGetter2 CreateConfListGetter() => new ConfigListGetter();
        #endregion

        #region Settings
        /// <summary>
        /// Create property class
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public PropertySettings CreateSettings(params KeyValuePair<string, string>[] paramList)
        {
            var prop = new PropertySettings(paramList);
            return prop;
        }

        #endregion

        public IProperty CreateProperty(ITarget target, IPropertyGetter2 propertyGetter, IPropertySettings settings = null)
        {
            var property = new ComplexProperty(target, propertyGetter, settings);
            return property;
        }

        public IProperty CreateProperty()
        {
            var property = new ComplexProperty(Target, PropertyGetter, PropertySettings);
            return property;
        }
        
        public string GetValue(string propName = null)
        {
            return Property.Value;
        }

    }

    /// <summary>
    /// Простой класс настроек для свойства
    /// </summary>
    public class PropertySettings : IPropertySettings
    {
        public PropertySettings() 
        {
            PropertyDict = new Dictionary<string, string>();
        }
        public PropertySettings(params KeyValuePair<string, string>[] paramList) 
        {
            PropertyDict = paramList.ToDictionary(key => key.Key, val => val.Value);
        }
        public Dictionary<string, string> PropertyDict;

        public string this[string param]
        {
            get
            {
                if (string.IsNullOrEmpty(param))
                    param = "value";
                string temp;
                PropertyDict.TryGetValue(param, out temp);
                return temp;
            }
            set
            {
                if (string.IsNullOrEmpty(param))
                    param = "value";
                PropertyDict[param] = value;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return PropertyDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
