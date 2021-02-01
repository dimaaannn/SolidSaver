using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.BaseTypes;
using SWAPIlib.MProperty.BaseProp;

namespace SWAPIlib.MProperty
{



    /// <summary>
    /// Базовый интерфейс управления текстовыми свойствами
    /// </summary>
    public interface IPropView : INotifyPropertyChanged
    {
        /// <summary>
        /// Доступно для чтения
        /// </summary>
        bool IsReadable { get; set; }
        /// <summary>
        /// Доступно для записи
        /// </summary>
        bool IsWritable { get; set; }
        /// <summary>
        /// Имя свойства
        /// </summary>
        string PropName { get; set; }
        /// <summary>
        /// Значение свойства
        /// </summary>
        string Value { get; set; }
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
        PropertyUpdate Update { get; set; }
        /// <summary>
        /// Записать значение
        /// </summary>
        PropertyWrite WriteValue { get; set; }
        void ClearSaved();
        void SetSavedVal(string s);
    }

}
