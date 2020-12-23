using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SWAPIlib.PropertyObj
{
    interface IPropertyUI
    {
        IEnumerable<IAppComponent> ComponentList { get; set; }
        string ConstructorName { get; set; }
        PropConstructor GetConstructor { get; }
        PropertyChanger PropChanger { get; set; }

        void ClearList();
        void LoadList();
    }

    class PropertyUI : IPropertyUI
    {
        public PropertyUI()
        {
            ConstructorDict = new Dictionary<string, PropConstructor>();
            ConstructorDict = new Dictionary<string, PropConstructor>()
            {
                {"Наименование", PropFactory.Nomination },
                {"Обозначение", PropFactory.DeNomination }
            };
            ConstructorName = ConstructorDict.Keys.First();

        }
        /// <summary>
        /// PropertyChanger object
        /// </summary>
        public PropertyChanger PropChanger
        {
            get
            {
                if (_prorChanger == null)
                {
                    _prorChanger = new PropertyChanger()
                    {
                        CaseSensitive = false,
                        UseRegExp = true
                    };
                }
                return _prorChanger;
            }
            set => _prorChanger = value;
        }
        private PropertyChanger _prorChanger;

        /// <summary>
        /// Заданное имя конструктора
        /// </summary>
        public string ConstructorName { get; set; }
        /// <summary>
        /// Текущий конструктор
        /// </summary>
        public PropConstructor GetConstructor
        {
            get
            {
                var result = ConstructorDict.TryGetValue(ConstructorName, out _getConstructor);
                if (result)
                    return _getConstructor;
                else
                    return PropFactory.Nomination; //default value
            }
        }
        private PropConstructor _getConstructor;
        /// <summary>
        /// Словарь конструкторов свойств
        /// </summary>
        private Dictionary<string, PropConstructor> ConstructorDict { get; set; }
        /// <summary>
        /// Список компонентов для генерации свойств
        /// </summary>
        public IEnumerable<IAppComponent> ComponentList { get; set; }

        /// <summary>
        /// Очистить список компонентов и объект свойств
        /// </summary>
        public void ClearList()
        {
            PropChanger.Components.Clear();
        }
        public void LoadList()
        {
            PropChanger.propConstructor = GetConstructor;

            foreach (var comp in ComponentList)
            {
                PropChanger.Components.Add(comp);
            }
        }

    }
}
