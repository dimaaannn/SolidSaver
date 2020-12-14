using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        void ProceedValues();
        void WriteValues();
        void RestoreValues();
        PropConstructor Propconstructor { get; set; }
        ValueConverter Valueconverter { get; set; }
        ObservableCollection<AppModel> Models { get; }
        ObservableCollection<ISwProperty> Properties { get; }
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

    class PropertyChangerBase : IPropertyChanger
    {

        public string SearchValue { get; set; }
        public string NewValue { get ; set; }
        public PropConstructor Propconstructor { get ;set; }
        public ValueConverter Valueconverter { get; set ; }

        public ObservableCollection<AppModel> Models => _Models;
        ObservableCollection<AppModel> _Models = new ObservableCollection<AppModel>();

        public ObservableCollection<ISwProperty> Properties => _Properties;
        ObservableCollection<ISwProperty> _Properties = new ObservableCollection<ISwProperty>();

        public PropertyChangerBase()
        {
            Models.CollectionChanged += Models_CollectionChanged;
        }

        private void Models_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (AppModel item in e.NewItems)
                {
                    //Properties.Add(Propconstructor(item));
                }
            }
        }

        public bool AllConfigurations { get; set; }

        public void ProceedValues()
        {
            foreach (var prop in Properties)
            {
                if(prop.IsReadable && prop.IsWritable)
                    prop.PropertyValue = Valueconverter(prop.PropertyValue);
            }
        }

        public void RestoreValues()
        {
            throw new NotImplementedException();
        }

        public void WriteValues()
        {
            throw new NotImplementedException();
        }

        
    }
}
