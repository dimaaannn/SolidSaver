using System;
using System.ComponentModel;
using System.Diagnostics;

namespace SWAPIlib.MProperty
{
    public class PropViewPassive : IPropView
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
        public string PropName { get; set; }

        #region Значения свойств
        protected string _NewPropertyValue;
        protected string _SavedPropertyValue;

        /// <summary>
        /// Значение свойства
        /// </summary>
        public string Value
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
                    return SavedValue;
                }
            }
            set
            {
                _NewPropertyValue = value;
                Debug.WriteLine($"AppPropertyBase - value was changed {_NewPropertyValue}");
                AllPropertyChanged();
            }
        }
        /// <summary>
        /// Значение до редактирования
        /// </summary>
        public string SavedValue
        {
            get
            {
                //Если значение отсутствует - обновить значение
                if (_SavedPropertyValue == null)
                {
                    Update();
                }
                return _SavedPropertyValue;
            }
            set
            {
                ClearValues();
                _SavedPropertyValue = value;
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
            UpdateVal?.Invoke(this, null);
        }
        /// <summary>
        /// Записать значение
        /// </summary>
        public virtual bool WriteValue()
        {
            var ret = false;
            WriteVal?.Invoke(this, _NewPropertyValue);
            //Вернуть true если значения совпадают
            if (SavedValue == _NewPropertyValue)
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
        public virtual void ClearValues()
        {
            _NewPropertyValue = null;
            AllPropertyChanged();
        }
        protected void AllPropertyChanged()
        {
            RaisePropertyChanged("ValueView");
            RaisePropertyChanged("CurrentValue");
            RaisePropertyChanged("IsModifyed");
        }
        /// <summary>
        /// Вызвать событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        public event EventHandler UpdateVal;
        public event EventHandler<string> WriteVal;

        protected void RaisePropertyChanged(string s)
        {
            var e = PropertyChanged;
            if (e != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(s));
            }
        }

    }
}
