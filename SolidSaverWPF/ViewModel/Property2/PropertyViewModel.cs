using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SWAPIlib.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel.Property2
{
    public class PropertyViewModel : ViewModelBase
    {
        private readonly string NullMessage = "<None>";
        private IProperty property;

        /// <summary>
        /// Ссылка на объект свойства
        /// </summary>
        public IProperty Property
        {
            get => property;
            set
            {
                property = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Имя свойства
        /// </summary>
        public string Name => Property?.Name ?? NullMessage;
        /// <summary>
        /// Возможность записи
        /// </summary>
        public bool IsWritable => Property?.IsReadOnly ?? false;
        public bool IsModifyed => Property?.TempValue != null;

        /// <summary>
        /// Записанное значение
        /// </summary>
        public string Value => Property?.TempValue ?? Property?.Value ?? NullMessage;
        /// <summary>
        /// Поле ввода пользователя
        /// </summary>
        public string UserValue
        {
            get => Property?.TempValue ?? Value;
            set
            {
                if (Property != null) Property.TempValue = value;
                RaisePropertyChanged("UserValue");
            }
        }

        /// <summary>
        /// Additional property list
        /// </summary>
        public Dictionary<string, string> ValuesList =>
            Property.ToDictionary(prop => prop.Key, prop => prop.Value);

        public void Update() { Property?.Update(); RaisePropertyChanged(); }
        public void WriteValue() { Property?.WriteValue(); RaisePropertyChanged(); }

        #region Commands
        private ICommand updatePropertyCommand;
        private ICommand writeValueCommand;
        private bool UpdateCommandCanExecute() => Property != null;
        private bool WriteValueCommandCanExecute() => Property?.IsReadOnly == true;

        public ICommand UpdatePropertyCommand => updatePropertyCommand ?? (updatePropertyCommand = new RelayCommand(Update, UpdateCommandCanExecute));

        public ICommand WriteValueCommand => writeValueCommand ?? (writeValueCommand = new RelayCommand(WriteValue, WriteValueCommandCanExecute));
        #endregion
    }
}
