using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.MessagesType;
using SWAPIlib.Property;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel.Property2
{
    public class PropertySetsViewModel : ViewModelBase
    {
        public PropertySetsViewModel() 
        {
            //Recieve all PropertySets
            MessengerInstance.Register<PropertyMessage>(this, RecievePropertyMessage);

        }

        private void RecievePropertyMessage(PropertyMessage obj)
        {
            if(obj.Action == PropertyMessage.PropAction.Show)
            {
                Properties.Clear();
                if(obj.PropertySet != null)
                {
                    foreach (IPropertySet propSet in obj.PropertySet)
                    {
                        Properties.Add(propSet);
                    }
                }
                RaisePropertyChanged("PropertyNameOptions");
            }
            else if(obj.Action == PropertyMessage.PropAction.Get 
                && obj.PropertySet == null)
            {
                obj.PropertySet = Properties;
            }
        }

        private ObservableCollection<IPropertySet> properties = new ObservableCollection<IPropertySet>();
        private string mainPropertyName;

        public ObservableCollection<IPropertySet> Properties { get => properties;
            set
            {
                Set(ref properties, value);
                MainPropertyName = null;
            }
        }

        /// <summary>
        /// Ключ отображаемого свойства
        /// </summary>
        public string MainPropertyName { 
            get => mainPropertyName;
            set
            {
                Set(ref mainPropertyName, value);
                foreach (var prop in Properties)
                {
                    prop.MainPropertyKey = mainPropertyName;
                }
            }
        }

        public IEnumerable<string> PropertyNameOptions =>
            Properties.Count > 0 ?
            Properties.First().Select(prop => prop.Key) : new List<string>() { "No data" };

        #region WriteUpdate
        /// <summary>
        /// Обновить свойства с ключом
        /// </summary>
        /// <returns></returns>
        public bool UpdateMains() => Properties.Select(prop => prop.Main.Update())
            .Aggregate((x, y) => x &= y);

        /// <summary>
        /// Записать свойства с ключом
        /// </summary>
        /// <returns></returns>
        public bool WriteMains() => Properties.Select(prop => WriteValueIfChanged(prop.Main))
            .Aggregate((x, y) => x &= y);

        /// <summary>
        /// Обновить все свойства
        /// </summary>
        public void UpdateAll()
        {
            var props = from sets in Properties
                        from prop in sets.Properties.Values
                        select prop;
            props.Select(cprop => cprop.Update()).ToList();
        } 
        #endregion

        /// <summary>
        /// Загрузить список свойств автоматически
        /// </summary>
        /// <param name="obj"></param>


        #region Commands
        private ICommand updateMainProp;
        private bool UpdateMainPropCanExecute() => 
            IsListNotEmpty()
            && !string.IsNullOrEmpty(MainPropertyName);
        public ICommand UpdateMainPropCommand => updateMainProp ?? (
            updateMainProp = new RelayCommand(() => UpdateMains(), UpdateMainPropCanExecute));

        private ICommand writeMainProp;
        private bool WriteMainPropCanExecute() => IsListNotEmpty() && !string.IsNullOrEmpty(MainPropertyName);
        public ICommand WriteMainPropCommand => writeMainProp ?? (
            writeMainProp = new RelayCommand(() => WriteMains(), WriteMainPropCanExecute));

        private ICommand updateAllProp;
        private bool UpdateAllPropCanExecute() => IsListNotEmpty();
        public ICommand UpdateAllPropCommand => updateAllProp ?? (
            updateAllProp = new RelayCommand(() => UpdateAll(), UpdateAllPropCanExecute));
        #endregion

        /// <summary>
        /// Запись производится только в случае изменения значения
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected bool WriteValueIfChanged(IProperty property)
        {
            bool ret = false;
            if (property?.TempValue != null)
            {
                ret = property.WriteValue();
            }
            else
                ret = true;
            return ret;
        }
        protected bool IsListNotEmpty() => Properties.Count > 0;
    }
}
