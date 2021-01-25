using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.BaseTypes;

namespace SWAPIlib.MProperty.PropObj
{
    public interface IAppPropertyBase : INotifyPropertyChanged 
    {
        /// <summary>
        /// Доступно для чтения
        /// </summary>
        bool IsReadable { get; }
        /// <summary>
        /// Доступно для записи
        /// </summary>
        bool IsWritable { get; }
        /// <summary>
        /// Имя свойства
        /// </summary>
        string PropertyName { get; }
        /// <summary>
        /// Значение свойства
        /// </summary>
        string PropertyValue { get; set; }
        /// <summary>
        /// Значение до редактирования
        /// </summary>
        string SavedValue { get; }
        /// <summary>
        /// Значение изменено
        /// </summary>
        bool IsModifyed { get; }
        /// <summary>
        /// Загрузить значение
        /// </summary>
        void Update();
        /// <summary>
        /// Записать значение
        /// </summary>
        bool WriteValue();
    }

    public interface AppProperty<T> : IAppPropertyBase
    {
        IPropGetter<T> propGetter { get; set; }

    }

    public class AppPropertyBase : IAppPropertyBase
    {

        /// <summary>
        /// Доступно для чтения
        /// </summary>
        public bool IsReadable { get; set; }
        /// <summary>
        /// Доступно для записи
        /// </summary>
        public bool IsWritable { get; set; }
        /// <summary>
        /// Имя свойства
        /// </summary>
        public string PropertyName { get; set; }

        #region Значения свойств
        protected string _NewPropertyValue;
        protected string _SavedPropertyValue;
        /// <summary>
        /// Значение свойства
        /// </summary>
        public string PropertyValue
        {
            get
            {
                //Если значение было получено
                if (_SavedPropertyValue != null)
                {
                    return _NewPropertyValue ?? _SavedPropertyValue;
                }
                //Прочитать значение в первый раз
                else
                {
                    Update();
                    return _SavedPropertyValue;
                }
            }
            set
            {
                _NewPropertyValue = value;
                Debug.WriteLine($"AppPropertyBase - value was changed {_NewPropertyValue}");
                RaisePropertyChanged("IsModifyed");
                RaisePropertyChanged("SavedValue");
                RaisePropertyChanged("PropertyValue");
            }
        }
        /// <summary>
        /// Значение до редактирования
        /// </summary>
        public string SavedValue
        {
            get
            {
                if (IsModifyed)
                    return _SavedPropertyValue;
                else 
                    return null;
            }
        }
        /// <summary>
        /// Значение было изменено
        /// </summary>
        public bool IsModifyed => _NewPropertyValue != null;
        #endregion

        /// <summary>
        /// Очистить временные значения
        /// </summary>
        public virtual void Update()
        {
            Debug.WriteLine("AppPropertyBase - update");
            _NewPropertyValue = null;
            RaisePropertyChanged("PropertyValue");
            RaisePropertyChanged("SavedValue");
            RaisePropertyChanged("IsModifyed");
        }
        /// <summary>
        /// Записать значение
        /// </summary>
        public virtual bool WriteValue()
        {
            _SavedPropertyValue = _NewPropertyValue;
            Update();
            return true;
        }

        /// <summary>
        /// Вызвать событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string s)
        {
            var e = PropertyChanged;
            if (e != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(s));
            }
        }

        //Добавить событие на изменения свойства и запись, добавить делегат чтения и записи
        //Продумать реализацию фабрики
    }
}
