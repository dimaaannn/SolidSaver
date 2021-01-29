using System;
using System.ComponentModel;
using System.Diagnostics;

namespace SWAPIlib.MProperty
{
    public class PropViewB : IPropView
    {
        public PropViewB() { }
        public PropViewB(IPropBinding binder) : this()
        {
            AttachBinderEvents(binder);
            PropBinder = binder;
        }

        private IPropBinding propBinder;
        public IPropBinding PropBinder 
        { 
            get => propBinder;
            set
            {
                DetachBinderEvents(propBinder);
                propBinder = value;
                AttachBinderEvents(propBinder);
            }
        }

        protected string _NewPropertyValue;
        protected string _SavedValue;

        /// <summary>
        /// Значение свойства
        /// </summary>
        public string Value
        {
            get => _NewPropertyValue ??  SavedValue;

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
                if (_SavedValue == null)
                {
                    Update();
                }
                return _SavedValue ?? null;
            }

        }
        /// <summary>
        /// Значение было изменено
        /// </summary>
        public bool IsModifyed => _NewPropertyValue != null;
        /// <summary>
        /// Доступно для чтения
        /// </summary>
        public bool IsReadable => PropBinder?.IsReadable ?? false;
        /// <summary>
        /// Доступно для записи
        /// </summary>
        public bool IsWritable => PropBinder?.IsWritable ?? false;
        /// <summary>
        /// Имя свойства
        /// </summary>
        public virtual string PropName => PropBinder?.DisplayPropName;

        /// <summary>
        /// Вызвать обновление
        /// </summary>
        public void Update()
        {
            Debug.WriteLine("PropertyTargetView - update");
            ClearValues();
            _SavedValue = PropBinder?.GetValue() ?? "ОШИБКА";
            if(PropBinder == null)
            {
                Debug.WriteLine("PropViewB - Null reference to binding");
            }
        }
        /// <summary>
        /// Записать значение
        /// </summary>
        public bool WriteValue()
        {
            var ret = false;
            //Записать
            ret = PropBinder?.SetValue(_NewPropertyValue) ?? false;

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
            _SavedValue = null;
            AllPropertyChanged();
        }
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

        protected virtual void AttachBinderEvents(IPropBinding binder)
        {
            if(binder != null)
            {
                binder.TargetUpdated += TargetUpdated;
                binder.WriteDataValue += WriteData;
                binder.FlushLocalData += FlushData;
                binder.TargetChanged += TargetChanged;
            }
        }
        protected virtual void DetachBinderEvents(IPropBinding binder)
        {
            if (binder != null)
            {
                binder.TargetUpdated -= TargetUpdated;
                binder.WriteDataValue -= WriteData;
                binder.FlushLocalData -= FlushData;
                binder.TargetChanged -= TargetChanged;
            }
        }

        /// <summary>
        /// оповещение об изменении свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public EventHandler<IDataEntity> TargetChanged => targetChanged;
        public EventHandler TargetUpdated => targetUpdated;
        public EventHandler WriteData => writeData;
        public EventHandler FlushData => flushData;


        protected virtual void targetChanged<T>(object sender, T newBind) 
        {
            if(sender is IPropBinding binder)
            {
                binder.TargetUpdated -= TargetUpdated;
                binder.WriteDataValue -= WriteData;
                binder.FlushLocalData -= FlushData;
            }
            if(newBind is IDataEntity de)
            {
                de.TargetUpdated += TargetUpdated;
                de.WriteData += WriteData;
                de.FlushData += FlushData;
            }
        }
        protected virtual void targetUpdated(object sender, EventArgs e) => Update();
        protected virtual void writeData(object sender, EventArgs e) => WriteValue();
        protected virtual void flushData(object sender, EventArgs e) => ClearValues();
    }
}
