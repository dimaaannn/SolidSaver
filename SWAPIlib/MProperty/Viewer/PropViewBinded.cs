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
                //Если значение отсутствует и доступно для чтения - обновить значение
                if (_SavedValue == null && IsReadable)
                {
                    Update();
                    AllPropertyChanged();
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
        public bool IsReadable => Update != null;
        /// <summary>
        /// Доступно для записи
        /// </summary>
        public bool IsWritable => WriteValue != null;
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
        /// <summary>
        /// оповещение об изменении свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
