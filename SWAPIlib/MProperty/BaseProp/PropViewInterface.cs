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
        string PropName { get; }
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
        void Update();
        /// <summary>
        /// Записать значение
        /// </summary>
        bool WriteValue();
        /// <summary>
        /// Очистить пользовательское значение
        /// </summary>
        void ClearValues();
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
        /// <summary>
        /// Событие обновления свойств
        /// </summary>
        EventHandler TargetUpdatedHandler { get; }
        /// <summary>
        /// Записать значения свойств
        /// </summary>
        EventHandler WriteValueHandler { get; }
        /// <summary>
        /// Очистить локальные свойства
        /// </summary>
        EventHandler FlushDataHandler { get; }

        /// <summary>
        /// Изменён объект привязки
        /// </summary>
        EventHandler<IDataEntity> TargetChanged { get; }
    }

    public interface IPropView<T> : IPropViewBase, IPropView
    {
        new IPropBinding<T> PropBinder { get; set; }

    }
}
