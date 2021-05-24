using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.PropertyBase
{
    public abstract class PropertyBase : IProperty, IPropertySettings
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyBase()
        {
            Logger.Trace("PropertyBase launch constructor");
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Возвратить значение
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value;
        /// <summary>
        /// Возвратить перечислитель
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary>
        /// Возможность записи новых значений
        /// </summary>
        public abstract bool IsReadOnly { get; }
        /// <summary>
        /// Имя свойства
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Пояснительная информация
        /// </summary>
        public abstract string Info { get; }
        /// <summary>
        /// Временное значение свойства
        /// </summary>
        public abstract string TempValue { get; set; }
        /// <summary>
        /// Значение для записи
        /// </summary>
        public abstract string Value { get; }

        public virtual string this[string param]
        {
            get => this.Value;
            set => TempValue = value;
        }

        /// <summary>
        /// Перечисление всех значений
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<KeyValuePair<string, string>> GetEnumerator();

        /// <summary>
        /// Прочитать значение заново
        /// </summary>
        /// <returns></returns>
        public abstract bool Update();
        /// <summary>
        /// Записать значение
        /// </summary>
        /// <returns></returns>
        public abstract bool WriteValue();
    }
}
