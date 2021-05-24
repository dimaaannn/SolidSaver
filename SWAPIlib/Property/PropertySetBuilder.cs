using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SWAPIlib.Property
{
    /// <summary>
    /// Строитель набора свойств для конкретного объекта согласно списку
    /// </summary>
    public class PropertySetBuilder2 : IEnumerable<KeyValuePair<string, IPropertyBuilder>>
    {
        public PropertySetBuilder2()
        {
            PropertyBuilders = new OrderedDictionary();
        }

        /// <summary>
        /// Словарь свойств
        /// </summary>
        public OrderedDictionary PropertyBuilders { get; }


        /// <summary>
        /// Основной объект привязки
        /// </summary>
        public ITarget Target { get; set; }
        /// <summary>
        /// Создать набор динамических свойств
        /// </summary>
        /// <param name="target"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public PropertySet Build(ITarget target)
        {
            var resultPropSet = new PropertySet(target);
            resultPropSet.UnionReplace(target.Settings);

            foreach (var keyVal in this)
            {
                string propertyKey = keyVal.Key;
                IPropertyBuilder builder = keyVal.Value;

                builder.Target = resultPropSet.Target;
                builder.Settings = builder.CreateSettings(resultPropSet);

                if (builder.IsPropertyValid)
                {
                    resultPropSet.Properties.Add(propertyKey, builder.Build());
                    builder.Settings = null;
                    builder.Target = null;
                }
                else
                    throw new MissingFieldException($"ComplexPropertySettingsBuilder: Property {keyVal} is not valid");
            }

            return resultPropSet;
        }

        #region Adders

        /// <summary>
        /// Добавить строителя в список
        /// </summary>
        /// <param name="name"></param>
        /// <param name="builder"></param>
        public void Add(string name, IPropertyBuilder builder) => PropertyBuilders.Add(name, builder);

        /// <summary>
        /// Создать строителя и добавить в список
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getter"></param>
        /// <param name="settings"></param>
        public void Add(string name, IPropertyGetter2 getter, IPropertySettings settings = null) =>
            Add(name, CreateNewBuilder(getter, settings));
        public void Add(string name, string value)
        {
            Add(name, new TextPropertyBuilder(name, value));
        }
        #endregion


        #region SupportMethods
        /// <summary>
        /// Создать текстовое поле, добавляемое в качестве опции
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected PropertyBuilder2 CreateNewBuilder(IPropertyGetter2 getter, IPropertySettings settings = null)
        {
            var builder = new PropertyBuilder2();
            builder.Getter = getter;
            builder.Settings = settings;

            return builder;
        }

        #endregion


        #region Enumerator
        public IEnumerator<KeyValuePair<string, IPropertyBuilder>> GetEnumerator()
        {
            var selection = from string key in PropertyBuilders.Keys
            select new KeyValuePair<string, IPropertyBuilder>(
                key
                ,(IPropertyBuilder) PropertyBuilders[key]
                );

            return selection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion
    }

    /// <summary>
    /// Строитель набора свойств для конкретного объекта согласно списку
    /// </summary>
    public class PropertySetBuilder : IEnumerable<KeyValuePair<string, IPropertyBuilder>>
    {
        public PropertySetBuilder()
        {
            PropertyBuilders = new Dictionary<string, IPropertyBuilder>();
        }

        /// <summary>
        /// Словарь свойств
        /// </summary>
        public Dictionary<string, IPropertyBuilder> PropertyBuilders { get; }
        /// <summary>
        /// Основной объект привязки
        /// </summary>
        public ITarget Target { get; set; }
        /// <summary>
        /// Создать набор динамических свойств
        /// </summary>
        /// <param name="target"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public PropertySet Build(ITarget target)
        {
            var propSet = new PropertySet(target);

            foreach (var keyValue in PropertyBuilders)
            {
                var propertyKey = keyValue.Key;
                var builder = keyValue.Value;
                builder.Target = propSet.Target;
                builder.CreateSettings(target.Settings);

                if (keyValue.Value.IsPropertyValid)
                {
                    propSet.Properties.Add(keyValue.Key, keyValue.Value.Build());
                }
                else
                    throw new MissingFieldException($"ComplexPropertySettingsBuilder: Property {keyValue.Key} is not valid");
            }

            return propSet;
        }

        #region Adders

        /// <summary>
        /// Добавить строителя в список
        /// </summary>
        /// <param name="name"></param>
        /// <param name="builder"></param>
        public void Add(string name, IPropertyBuilder builder) => PropertyBuilders.Add(name, builder);

        /// <summary>
        /// Создать строителя и добавить в список
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getter"></param>
        /// <param name="settings"></param>
        public void Add(string name, IPropertyGetter2 getter, IPropertySettings settings = null) =>
            Add(name, CreateNewBuilder(getter, settings));
        public void Add(string name, string value)
        {
            Add(name, new TextPropertyBuilder(name, value));
        }
        #endregion


        #region SupportMethods
        /// <summary>
        /// Создать текстовое поле, добавляемое в качестве опции
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected PropertyBuilder2 CreateNewBuilder(IPropertyGetter2 getter, IPropertySettings settings = null)
        {
            var builder = new PropertyBuilder2();
            builder.Getter = getter;
            builder.Settings = settings;

            return builder;
        }

        #endregion


        #region Enumerator
        public IEnumerator<KeyValuePair<string, IPropertyBuilder>> GetEnumerator() => PropertyBuilders.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator(); 
        #endregion
    }
}
