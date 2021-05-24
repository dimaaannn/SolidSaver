using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Property
{
    public class ComplexValue : IComplexValue, IPropertySettings
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Конструктор
        /// </summary>
        public ComplexValue(string name)
        {
            Logger.Trace("simple ComplexValue created");
            ValuePairs = new Dictionary<string, string>();
            PropertyName = name;
        }

        /// <summary>
        /// Сохранённые свойства
        /// </summary>
        public Dictionary<string, string> ValuePairs { get; }

        public string PropertyName { get; set; }
        public string BaseValue
        {
            get => GetValue();
            set
            {
                if (string.IsNullOrEmpty(PropertyName))
                    PropertyName = "value";
                SetValue(PropertyName, value);
            }
        }

        public string GetValue(string propName = null)
        {

            //По умолчанию ключом является базовое имя свойства
            if (string.IsNullOrEmpty(propName))
            {
                propName = PropertyName;
            }

            //Если в списке всего одно значение - выбрать его установить как propname
            if (string.IsNullOrEmpty(propName))
            {
                //Получить первый случайный ключ
                if (ValuePairs.Count == 1)
                {
                    var firstKey = ValuePairs.Keys.First();
                    Logger.Info("ComplexValue: {name} Выбран первый случайный ключ {Key}", PropertyName, firstKey);
                    propName = firstKey;
                }
                else
                    return "<СПИСОК ЗНАЧЕНИЙ>";
            }

            if (ValuePairs.ContainsKey(propName))
            {
                var ret = ValuePairs[propName];
                Logger.Trace("ComplexValue return named property {property} = {val}", propName, ret);
                return ret;
            }
            else
            {
                Logger.Trace("ComplexValue property {property} not found", propName);
                return null;
            }
        }

        public void SetValue(string propName, string newVal)
        {
            if (string.IsNullOrEmpty(propName))
                propName = PropertyName;
            if (ValuePairs.ContainsKey(propName))
                ValuePairs[propName] = newVal;
            else
                ValuePairs.Add(propName, newVal);
            Logger.Info("ComplexValue set temp {property} to {value}", propName, newVal);
        }

        #region Interface implementation
        /// <summary>
        /// Обработка именованных значений
        /// </summary>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public string this[string valueName]
        {
            get => GetValue(valueName);
            set => SetValue(valueName, value);
        }

        /// <summary>
        /// Return dictionary data
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() =>
            ValuePairs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
        //Merge 2 dict
        //Dictionary<string, string> d6 = d3.Union(d1.Where(x => !d3.Keys.Contains(x.Key))).ToDictionary(k => k.Key, v => v.Value);
    }

}
