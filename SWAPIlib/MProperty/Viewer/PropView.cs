using System;
using System.ComponentModel;
using System.Diagnostics;
using SWAPIlib.MProperty.BaseProp;

namespace SWAPIlib.MProperty
{
    public class PropView : IPropView
    {

        protected string _NewPropertyValue;
        protected string _SavedValue;

        /// <summary>
        /// Значение свойства
        /// </summary>
        public string Value
        {
            get => _NewPropertyValue ?? SavedValue;

            set
            {
                _NewPropertyValue = value;
                Debug.WriteLine($"AppPropertyBase - value was changed {_NewPropertyValue}");
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
                if (_SavedValue == null)
                {
                    Update?.Invoke();
                }
                return _SavedValue;
            }
        }
        /// <summary>
        /// Значение было изменено
        /// </summary>
        public bool IsModifyed => _NewPropertyValue != null;
        /// <summary>
        /// Доступно для чтения
        /// </summary>
        public bool IsReadable  { get; set; }
        /// <summary>
        /// Доступно для записи
        /// </summary>
        public bool IsWritable { get; set; }
        /// <summary>
        /// Имя свойства
        /// </summary>
        public string PropName { get; set; }

        /// <summary>
        /// Вызвать обновление
        /// </summary>
        public PropertyUpdate Update { get; set; }
        /// <summary>
        /// Записать значение
        /// </summary>
        public PropertyWrite WriteValue { get; set; }

        protected void AllPropertyChanged()
        {
            RaisePropertyChanged("ValueView");
            RaisePropertyChanged("CurrentValue");
            RaisePropertyChanged("IsModifyed");
        }
        /// <summary>
        /// Прокси для вызова обновления значений
        /// </summary>
        /// <param name="s"></param>
        protected void RaisePropertyChanged(string s)
        {
            var e = PropertyChanged;
            if (e != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(s));
            }
        }

        public void ClearSaved()
        {
            _SavedValue = null;
            _NewPropertyValue = null;
            AllPropertyChanged();
        }
        /// <summary>
        /// Установить обновлённое значение
        /// </summary>
        /// <param name="s"></param>
        public void SetSavedVal(string s)
        {
            _NewPropertyValue = null;
            _SavedValue = s;
            AllPropertyChanged();
        }

        /// <summary>
        /// оповещение об изменении свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
