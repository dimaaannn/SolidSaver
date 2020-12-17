using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib;

namespace SWAPIlib.PropertyObj
{
    public delegate ISwProperty PropConstructor(AppModel appModel, string ConfigName);
    public delegate string ValueConverter(string Value);

    public interface IPropertyChanger
    {
        string SearchValue { get; set; }
        string NewValue { get; set; }
        bool UseRegExp { get; set; }
        PropConstructor propConstructor {get;set;}
        void ProceedValues();
        void WriteValues();
        void RestoreValues();

        ITextReplacer textReplacer { get; }
        ObservableCollection<IAppComponent> Components { get; }
        ObservableCollection<IPropModifier> Properties { get; }
        bool AllConfigurations { get; set; }
        
    }

    public interface IPropModifier
    {
        /// <summary>
        /// Имя детали
        /// </summary>
        string PartName { get; }
        /// <summary>
        /// Модель детали
        /// </summary>
        AppModel Model { get; set; }
        /// <summary>
        /// Список конфигураций
        /// </summary>
        ObservableCollection<string> ConfigNames { get; set; }
        /// <summary>
        /// Словарь старых значений
        /// </summary>
        Dictionary<string, string> OldValues { get; }
        /// <summary>
        /// Словарь объектов свойств
        /// </summary>
        Dictionary<string, ISwProperty> SwPropList { get; set; }
        /// <summary>
        /// Конструктор свойств
        /// </summary>
        PropConstructor propConstructor { get; set; }
        /// <summary>
        /// Записать новые значения
        /// </summary>
        void WriteValues();
        /// <summary>
        /// Восстановить значения
        /// </summary>
        void RestoreValues();
        /// <summary>
        /// Показать все конфигурации
        /// </summary>
        bool AllConfiguration { get; set; }

    }

    public class PropertyChanger : IPropertyChanger
    {
        public PropertyChanger()
        {
            textReplacer = new TextReplacer() { UseRegExp = false };
            Components = new ObservableCollection<IAppComponent>();
            Properties = new ObservableCollection<IPropModifier>();
            Components.CollectionChanged += Components_AddRemoveItem;
        }


        public string SearchValue 
        { 
            get => textReplacer.SearchText; 
            set => textReplacer.SearchText = value; 
        }
        public string NewValue
        {
            get => textReplacer.ReplaceText;
            set => textReplacer.ReplaceText = value;
        }
        public bool UseRegExp
        {
            get => textReplacer.UseRegExp;
            set => textReplacer.UseRegExp = value;
        }

        public PropConstructor propConstructor { get; set; }
        public ITextReplacer textReplacer { get; private set; }
        public ObservableCollection<IAppComponent> Components { get; private set; }
        public ObservableCollection<IPropModifier> Properties { get; private set; }

        public bool AllConfigurations
        {
            get => _allConfigurations;
            set
            {
                _allConfigurations = value;
                foreach (var propModif in Properties)
                {
                    propModif.AllConfiguration = _allConfigurations;
                }
            }
        }
        private bool _allConfigurations = false;

        public void ProceedValues()
        {
            var allProps = from propmod in Properties
                           from prop in propmod.SwPropList
                           select prop.Value;

            foreach (var prop in allProps)
            {
                textReplacer.Replace(prop.PropertyValue);
                if (textReplacer.IsReplaced)
                    prop.PropertyValue = textReplacer.ReplaceResult;

            }
        }

        public void RestoreValues()
        {
            foreach (var prop in Properties)
            {
                prop.RestoreValues();
            }
        }

        public void WriteValues()
        {
            foreach (var prop in Properties)
            {
                prop.WriteValues();
            }
        }

        protected void AddComponent(AppComponent component)
        {
            //Null part referece check
            if (component.IsExist)
            {

                var newProp = new PropModifier(component.PartModel, propConstructor)
                { AllConfiguration = this.AllConfigurations };

                //Add current active configuration
                newProp.ConfigNames.Add(component.RefConfigName);
                Properties.Add(newProp);
            }
            else
            {
                Debug.WriteLine($"Null reference model in component{component.Title}");
                //TODO Создать действие на погашенный компонент
            }
        }
        protected void RemoveComponent(AppComponent component)
        {
            var removedProperty = Properties.Where(prop => prop.Model == component.PartModel);
            foreach(var prop in removedProperty)
            {
                Properties.Remove(prop);
            }
        }
        private void Components_AddRemoveItem(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var newComp = e.NewItems[0] as AppComponent;
                var EqualsCounter = Components.Where(comp => comp.Equals(newComp)).Count();
                if (EqualsCounter <= 1)
                    AddComponent(newComp);
                else
                    Debug.WriteLine($"Comp {newComp} alredy in list");
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                RemoveComponent(e.OldItems[0] as AppComponent);
            }
        }

    }
}
