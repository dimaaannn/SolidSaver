using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.BaseTypes;

namespace SWAPIlib.MProperty
{
    /// <summary>
    /// Базовый интерфейс управления текстовыми свойствами
    /// </summary>
    public interface IPropViewBase : INotifyPropertyChanged
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
        string Title { get; }
        /// <summary>
        /// Значение свойства
        /// </summary>
        string MainValueView { get; set; }
        /// <summary>
        /// Значение до редактирования
        /// </summary>
        string CurrentValue { get; }
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
        event EventHandler<string> WriteVal;
    }

    /// <summary>
    /// Управление свойством объекта привязки
    /// </summary>
    public interface IPropView : IPropViewBase
    {
        /// <summary>
        /// Объект привязки свойства
        /// </summary>
        IPropBinding PropBinder { get; set; }
    }
}
