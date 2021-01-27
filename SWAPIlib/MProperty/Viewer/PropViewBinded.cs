using System;
using System.ComponentModel;
using System.Diagnostics;

namespace SWAPIlib.MProperty
{
    public class PropViewB : IPropView
    {
        public PropViewB() { }
        public PropViewB(IPropBinding binder) :this()
        {
            PropBinder = binder;
        }

        private IPropBinding propBinder;
        public IPropBinding PropBinder { get => propBinder; set => propBinder = value; }

        /// <summary>
        /// Доступно для чтения
        /// </summary>
        public bool IsReadable => PropBinder.IsReadable;
        /// <summary>
        /// Доступно для записи
        /// </summary>
        public bool IsWritable => PropBinder.IsWritable;
        /// <summary>
        /// Имя свойства
        /// </summary>
        public virtual string Title => PropBinder.Title;

        #region Значения свойств
        protected string _NewPropertyValue;
        protected string _SavedPropertyValue;

        /// <summary>
        /// Значение свойства
        /// </summary>
        public string MainValueView
        {
            get => _NewPropertyValue ?? CurrentValue;

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
        public string CurrentValue
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

        }
        /// <summary>
        /// Значение было изменено
        /// </summary>
        public bool IsModifyed => _NewPropertyValue != null;
        #endregion

        /// <summary>
        /// Вызвать обновление
        /// </summary>
        public void Update()
        {
            Debug.WriteLine("PropertyTargetView - update");
            UpdateVal?.Invoke(this, null);
            ClearValues();
            _SavedPropertyValue = PropBinder.GetValue();
        }
        /// <summary>
        /// Записать значение
        /// </summary>
        public bool WriteValue()
        {
            var ret = false;
            WriteVal?.Invoke(this, null);
            //Записать
            ret = PropBinder.SetValue(_NewPropertyValue);

            //Debug info
            if (ret)
                Debug.WriteLine($"AppPropertyView: Значение {_NewPropertyValue} записано");
            else
                Debug.WriteLine($"AppPropertyView: Значение {_NewPropertyValue} записано");

            ClearValues();

            return ret;
        }
        /// <summary>
        /// Сбросить значения пользователя
        /// </summary>
        public void ClearValues()
        {
            _NewPropertyValue = null;
            _SavedPropertyValue = null;
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
