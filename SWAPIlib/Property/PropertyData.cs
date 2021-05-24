using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Property
{

    //УСТАРИЕЛО, заменено на PropertyView
    /// <summary>
    /// Набор логически связанных между собой свойств
    /// </summary>
    public class PropertyData : IEnumerable<IProperty>
    {
        private List<IProperty> propertyList;

        public PropertyData(ITarget target = null, params IProperty[] property)
        {
            propertyList = new List<IProperty>(property);
            var first = property.First();
            NameProperty = GetName(target);
            AdditionalProperty = GetAdditionalData(target);
        }

        /// <summary>
        /// Свойство по умолчанию
        /// </summary>
        public IProperty Main => PropertyList.First();
        /// <summary>
        /// Список свойств
        /// </summary>
        public List<IProperty> PropertyList { get => propertyList; protected set => propertyList = value; }
        /// <summary>
        /// Имя набора
        /// </summary>
        public IProperty NameProperty { get; protected set; }
        /// <summary>
        /// Дополнительное поле информации
        /// </summary>
        public IProperty AdditionalProperty { get; protected set; }

        /// <summary>
        /// Добавить свойство в список
        /// </summary>
        /// <param name="property"></param>
        public void Add(IProperty property) => PropertyList.Add(property);
        /// <summary>
        /// Удалить свойство из списка
        /// </summary>
        /// <param name="property"></param>
        public void Remove(IProperty property) => PropertyList.Remove(property);
        /// <summary>
        /// Записать значения
        /// </summary>
        /// <returns></returns>
        public bool Write() => PropertyList
            .Select(prop => prop.WriteValue())
            .Aggregate((x, y) => x & y);
        /// <summary>
        /// Обновить значения
        /// </summary>
        /// <returns></returns>
        public bool Update() => PropertyList
            .Select(prop => prop.Update())
            .Aggregate((x, y) => x & y);

        protected virtual IProperty GetName(ITarget target)
        {
            return new TextProperty("testName", "testValue");
        }

        protected virtual IProperty GetAdditionalData(ITarget target)
        {
            return new TextProperty("testadd prop", "test add Value");
        }

        public override string ToString()
        {
            return Main.ToString();
        }

        public IEnumerator<IProperty> GetEnumerator() => PropertyList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => PropertyList.GetEnumerator();
    }


    //internal static class InfoCreator
    //{
    //    public static IProperty GetName(ITarget target)
    //    {
    //        IProperty ret = null;
    //        switch (target.GetTarget())
    //        {
    //            case IAppComponent comp:
    //                ret = new PropertyBuilder() { Target = comp, PropertyGetter}

    //            default:
    //                break;
    //        }
    //        return ret;
    //    }
    //}
}
