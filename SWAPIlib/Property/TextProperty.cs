using System.Collections.Generic;

namespace SWAPIlib.Property
{
    public class TextProperty : SWAPIlib.Property.PropertyBase.PropertyBase
    {
        private readonly string name;
        private readonly bool isWritable;
        protected string savedValue;
        protected string tempValue;
        protected string info;

        public TextProperty(string propertyName, string propValue, bool isWritable = true) : base()
        {
            name = propertyName;
            savedValue = propValue;
            Logger.Info("TextProperty {name} created with value: {value}", Name, savedValue);
            this.isWritable = isWritable;
        }

        public override string Name => name;
        public override bool IsReadOnly => !isWritable;
        public override string Info => info;
        public override string TempValue
        {
            get
            {
                Logger.Trace("TextProperty {name} return temp value: {tempvalue}", Name, tempValue);
                return tempValue;
            }
            set
            {
                Logger.Info("TextProperty {name} set temp Value:", Name, value);
                tempValue = value;
                OnPropertyChanged("TempValue");
            }
        }
        public override string Value
        {
            get
            {
                Logger.Trace("TextProperty {name} return value: {value}", Name, savedValue);
                if (savedValue == null)
                    Logger.Warn("TextProperty {name} return null", Name);
                return savedValue;
            }
        }

        /// <summary>
        /// Сохранить временное значение как постоянное
        /// </summary>
        /// <returns></returns>
        public override bool WriteValue()
        {
            Logger.Info("TextProperty {name} WriteValue replace old value: {value} with tempValue: {tempValue}", Name, savedValue, tempValue);
            savedValue = tempValue;
            tempValue = null;
            OnPropertyChanged("Value");
            OnPropertyChanged("TempValue");
            return true;
        }
        /// <summary>
        /// Очистить временное значение
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            Logger.Info("TextProperty {name} Update: {value}, tempval {tempVal} cleared", Name, savedValue, tempValue);
            tempValue = null;
            OnPropertyChanged("Value");
            OnPropertyChanged("TempValue");
            return true;
        }

        /// <summary>
        /// Заглушка для перечислителя
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return new Dictionary<string, string>() { { Name, Value } }.GetEnumerator();
        }
    }
}