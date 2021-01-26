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
    public interface IPropertyView : INotifyPropertyChanged 
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
        string Title { get; set; }
        /// <summary>
        /// Значение свойства
        /// </summary>
        string ValueView { get; set; }
        /// <summary>
        /// Значение до редактирования
        /// </summary>
        string CurrentValue { get; set; }
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
        /// <summary>
        /// Очистить пользовательское значение
        /// </summary>
        void ClearValues();

        /// <summary>
        /// Запрос обновления
        /// </summary>
        event EventHandler UpdateVal;
        /// <summary>
        /// Запрос записи
        /// </summary>
        event EventHandler WriteVal;
    }


    public class PropertyView : IPropertyView
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
        public string Title { get; set; }

        #region Значения свойств
        protected string _NewPropertyValue;
        protected string _SavedPropertyValue;
        /// <summary>
        /// Значение свойства
        /// </summary>
        public string ValueView
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
                    return CurrentValue;
                }
            }
            set
            {
                _NewPropertyValue = value;
                Debug.WriteLine($"AppPropertyBase - value was changed {_NewPropertyValue}");
                ClearValues();
            }
        }
        /// <summary>
        /// Значение до редактирования
        /// </summary>
        public string CurrentValue
        {
            get
            {
                //Если значение отсутствует - обновить значение
                if(_SavedPropertyValue == null)
                {
                    Update();
                }
                return _SavedPropertyValue;
            }
            set
            {
                _SavedPropertyValue = value;
                ClearValues();
            }

        }
        /// <summary>
        /// Значение было изменено
        /// </summary>
        public bool IsModifyed => _NewPropertyValue != null;
        #endregion

        /// <summary>
        /// Вызвать обновление
        /// </summary>
        public virtual void Update()
        {
            Debug.WriteLine("AppPropertyBase - update");
            UpdateVal.Invoke(this, null);
        }
        /// <summary>
        /// Записать значение
        /// </summary>
        public virtual bool WriteValue()
        {
            var ret = false;
            WriteVal.Invoke(this, null);
            //Вернуть true если значения совпадают
            if (CurrentValue == _NewPropertyValue)
            {
                ret = true;
                Debug.WriteLine($"AppPropertyView: Значение {_NewPropertyValue} записано");
                //Очистить значение
                ClearValues();
            }
            else
            {
                Debug.WriteLine($"AppPropertyView: Значение {_NewPropertyValue} записано");
            }

            return ret;
        }
        /// <summary>
        /// Сбросить значения пользователя
        /// </summary>
        public void ClearValues()
        {
            _NewPropertyValue = null;
            RaisePropertyChanged("ValueView");
            RaisePropertyChanged("CurrentValue");
            RaisePropertyChanged("IsModifyed");
        }
        /// <summary>
        /// Вызвать событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        public event EventHandler UpdateVal;
        public event EventHandler WriteVal;

        protected void RaisePropertyChanged(string s)
        {
            var e = PropertyChanged;
            if (e != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(s));
            }
        }

        //Продумать реализацию фабрики
    }
}
