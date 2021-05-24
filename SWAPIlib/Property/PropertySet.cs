using SWAPIlib.Property.ModelProperty;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property
{
    public interface IPropertySet : IEnumerable<KeyValuePair<string, IProperty>>
    {
        IProperty this[string param] { get; set; }

        /// <summary>
        /// Имя объекта привязки
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Ключ основного свойства
        /// </summary>
        string MainPropertyKey { get; set; }

        /// <summary>
        /// Базовое свойство взаимодействия
        /// </summary>
        IProperty Main { get; }

        Dictionary<string, IProperty> Properties { get; }
        ITarget Target { get; }

        void AddTextProperty(string key, string value);
        void UnionPreserve(IPropertySettings settings);
        void UnionReplace(IPropertySettings settings);
    }


    public class PropertySet : IPropertySet, IPropertySettings
    {
        private string mainPropertyKey;

        private PropertySet()
        {
            Properties = new Dictionary<string, IProperty>();
        }

        public PropertySet(ITarget target, string mainPropName = null) : this()
        {
            Properties = new Dictionary<string, IProperty>();
            Target = target;
        }

        public string Name => Target.TargetName;
        public string MainPropertyKey 
        { 
            get => mainPropertyKey ?? Properties.First().Key; 
            set => mainPropertyKey = value; 
        }

        public Dictionary<string, IProperty> Properties { get; protected set; }
        /// <summary>
        /// Объект связанный с набором свойств
        /// </summary>
        public ITarget Target { get; }

        public IProperty Main => this[MainPropertyKey];

        /// <summary>
        /// Дополнить список свойств, перезаписав старые
        /// </summary>
        /// <param name="settings"></param>
        public void UnionReplace(IPropertySettings settings)
        {
            if (settings is PropertySet complexSettings)
            {
                Properties = Properties.Concat(complexSettings.Properties)
                  .GroupBy(kvp => kvp.Key, kvp => kvp.Value)
                  .ToDictionary(g => g.Key, g => g.Last());
            }
            else if (settings != null)
                foreach (var keyVal in settings)
                    AddTextProperty(keyVal.Key, keyVal.Value);
        }
        public void UnionPreserve(IPropertySettings settings)
        {
            if (settings is PropertySet complexSettings)
            {
                Properties = Properties.Concat(complexSettings.Properties)
                  .GroupBy(kvp => kvp.Key, kvp => kvp.Value)
                  .ToDictionary(g => g.Key, g => g.First());
            }
            else if (settings != null)
                foreach (var keyVal in settings)
                {
                    if (!Properties.ContainsKey(keyVal.Key))
                        AddTextProperty(keyVal.Key, keyVal.Value);
                }
        }

        public void AddTextProperty(string key, string value)
        {
            Properties.Add(key, new TextProperty(key, value));
        }

        public IProperty this[string param]
        {
            get
            {
                if (Properties.ContainsKey(param))
                    return Properties[param];
                else return null;
            }
            set => Properties.Add(param, value);
        }
        string IPropertySettings.this[string param]
        {
            get
            {
                if (Properties.ContainsKey(param))
                    return Properties[param].Value;
                else return null;
            }
            //set => Properties.Add(param, new TextProperty(param, value));
        }

        /// <summary>
        /// Получает свойство из словаря или возвращает Null
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <returns></returns>
        private IProperty GetPropertyWithName(string propertyName)
        {
            if (Properties.ContainsKey(propertyName))
            {
                return Properties[propertyName];
            }
            else
                return null;
        }

        #region Ienumerable
        public IEnumerator<KeyValuePair<string, IProperty>> GetEnumerator() => Properties.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() =>
            Properties.ToDictionary(key => key.Key, key => key.Value.Value).GetEnumerator();

        #endregion

        public override string ToString() => $"{Name} propertySet";
    }

}
