using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property
{
    public interface IProperty
    {
        /// <summary>
        /// Объект привязки
        /// </summary>
        ITargetInteraction Target { get; }
        /// <summary>
        /// Обработчик свойства
        /// </summary>
        IPropertyGetter2 Getter { get; }
        /// <summary>
        /// Значение свойства
        /// </summary>
        IInteractionValue PropertyValue { get; }
        /// <summary>
        /// Несохранённые изменения
        /// </summary>
        bool IsModifyed { get; }

        /// <summary>
        /// Значение
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Обновить значение
        /// </summary>
        /// <returns></returns>
        bool Update();
        /// <summary>
        /// Записать новое значение
        /// </summary>
        /// <returns></returns>
        bool WriteValue();
        /// <summary>
        /// Сбросить изменения
        /// </summary>
        void ClearSaved();
    }



    /// <summary>
    /// Базовый интерфейс управления текстовыми свойствами
    /// </summary>
    public interface IPropView2 : INotifyPropertyChanged
    {
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
        Action Update { get; set; }
        /// <summary>
        /// Записать значение
        /// </summary>
        Func<string, bool> WriteValue { get; set; }
        void ClearSaved();
        void WriteUserValue();
    }


    /// <summary>
    /// Привязка к объекту для получения и задания свойств
    /// </summary>
    public interface IPropertyGetter2
    {
        /// <summary>
        /// Совместимые цели
        /// </summary>
        TargetType CompatibleTypes { get; }
        /// <summary>
        /// Идентификатор типа свойства
        /// </summary>
        PropertyID PropertyID { get; }
        /// <summary>
        /// Описание свойства
        /// </summary>
        string PropertyInfo { get; }
        /// <summary>
        /// Проверить совместимость с целью
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool CheckTarget(ITargetInteraction target);
        /// <summary>
        /// Прочитать значение переменной
        /// </summary>
        /// <returns></returns>
        bool GetValue(ITargetInteraction target);
        /// <summary>
        /// Записать значение
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        bool SetValue(ITargetInteraction target);
    }

    /// <summary>
    /// Объект с информацией о свойстве и настройками
    /// </summary>
    public interface ITargetInteraction
    {
        TargetType TargetType { get; }
        string TargetName { get; }
        string[] TargetInfo { get; }
        IDictionary<string, string> TargetOptions { get; set; }
        IDictionary<string, string> PropertyOptions { get; set; }
        /// <summary>
        /// Полученные данные
        /// </summary>
        IInteractionValue PropertyValue { get; }
    }

    /// <summary>
    /// Полученные данные
    /// </summary>
    public interface IInteractionValue
    {
        bool IsReadable { get; }
        bool IsWritable { get; }
        /// <summary>
        /// Расширенная информация о свойстве
        /// </summary>
        string[] PropertyInfo { get; }
        /// <summary>
        /// Последнее полученное значение свойства
        /// </summary>
        string Current { get; }
        /// <summary>
        /// Значение для записи
        /// </summary>
        string UserEdit { get; set; }
    }

    /// <summary>
    /// Тип цели для проверки совместимости свойств
    /// </summary>
    [Flags]
    public enum TargetType
    {
        None = 0,
        File = 1,
        Component = 1 << 1,
        Assembly =  1 << 2,
        Part =      1 << 3
    }

    public enum PropertyID
    {
        None = 0,
        PartAppereance = 1
    }
}
