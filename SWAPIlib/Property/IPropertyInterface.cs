using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Интерфейсы для объекта обработки свойств:
 * 
 * IProperty - базовый класс для доступа к описанию, имени и значению свойства
 * Кэширует данные при первом обращении, для последующего обновления требуется вызывать
 * метод Update() - очищает кэш и временное значение
 * Запись значения происходит из кэша при помощи соотв.метода. Временное значение и кэш очищаются.
 * Флаг IsWritable показывает опцию записи у конкретного обработчика.
 * Метод ToString возвращает значение свойства. 
 * Перечислитель добавлен для доступа к расширениям
 * 
 * 
 * IComplexProperty - расширенный интерфейс IProperty для более сложных свойств:
 * 
 * Target - Класс-обёртка для объекта из которого будет браться свойство. 
 * Содержит в себе имя, тип, и информацию об объекте внутри. 
 * 
 * ResultValue - Возвращает значение свойства в виде объекта IComplexValue (его созданием занимается обработчик)
 * В результате содержится словарь, где ключом является имя сущности (Имя конфигурации, имя файла, и т.п) а значение представляет строку с данными
 * Кэширование значения и запись производится аналогично базовому классу, но для записи требуется создать объект IComplexValue, где так же требуется указать сущность поддерживаемую конкретным обработчиком.
 * При обращении через TempComplexValue объект создаётся автоматически.
 * 
 * Value - возвращает одно значение по умолчанию, в случае неоднозначности 
 * возвращает текст <СПИСОК ЗНАЧЕНИЙ> (Конкретная реализация зависит от дочерних классов)
 * По факту - является заглушкой.
 * 
 * PropertySettings - Объект аггрегатор пар ключ-значение, 
 * в качестве ключа используется то же имя сущности, а в качестве значения - любой объект с методом ToString(); Предназначен для для комплексных свойств, где результат может зависеть от значения других свойств. При каждой записи или обновлении передаётся опциональным параметром обработчику.
 * 
 *
 *IPropertyGetter - Обработчик для объекта ITarget, реализует только ReadOnly поля и методы.
 *Содержит внутри себя логику получения значения свойства. 
 *
 *GetterType - содержит ключи с информацией о возможности записи, чтения, и т.п.
 *TargetType - Ключи поддерживаемых типов ITarget
 *
 *В поле Name содержит сущность возвращаемого (или записываемого) значения.
 *
 *OptionsRequirement содержит обязательные параметры IPropertySettings для конкретного обработчика
 *
 */


namespace SWAPIlib.Property
{
    #region Property
    /// <summary>
    /// Глобальное свойство
    /// </summary>
    public interface IProperty : IEnumerable<KeyValuePair<string, string>>, INotifyPropertyChanged
    {
        /// <summary>
        /// Доступен для записи
        /// </summary>
        bool IsReadOnly { get; }
        /// <summary>
        /// Подсказка
        /// </summary>
        string Info { get; }
        /// <summary>
        /// Имя свойства
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Значение для изменений
        /// </summary>
        string TempValue { get; set; }
        /// <summary>
        /// Прочитанное значение
        /// </summary>
        string Value { get; }

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
    }

    /// <summary>
    /// Свойство с возможностью настроек и опций
    /// </summary>
    public interface IComplexProperty : IProperty
    {
        /// <summary>
        /// Отсутствие ошибок
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// Объект привязки
        /// </summary>
        ITarget Target { get; }
        /// <summary>
        /// Объект значений
        /// </summary>
        IComplexValue ResultValue { get; }
        /// <summary>
        /// Объект для записи значений
        /// </summary>
        IComplexValue TempComplexValue { get; set; }
        /// <summary>
        /// Настройки опций при получении и установке значений
        /// </summary>
        PropertySet PropertySettings { get; }

        /// <summary>
        /// Архив свойств
        /// </summary>
        Dictionary<object, List<WeakReference<IComplexProperty>>> PropertyList { get; }
    }

    #endregion


    #region Getter
    /// <summary>
    /// Привязка к объекту для получения и задания свойств
    /// </summary>
    public interface IPropertyGetter2
    {
        /// <summary>
        /// Совместимые типы цели
        /// </summary>
        TargetType TargetType { get; }
        GetterType GetterType { get; }
        /// <summary>
        /// Уникальное имя свойства
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Описание свойства
        /// </summary>
        string Info { get; }
        /// <summary>
        /// Проверить совместимость с целью
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool CheckTarget(ITarget target, IPropertySettings settings);
        /// <summary>
        /// Прочитать значение переменной
        /// </summary>
        /// <returns></returns>
        IComplexValue GetValue(ITarget target, IPropertySettings settings);
        /// <summary>
        /// Записать значение
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        bool SetValue(ITarget target, IComplexValue newValue, IPropertySettings settings);
        /// <summary>
        /// Список имён необходимых параметров
        /// </summary>
        HashSet<string> OptionsRequirement { get; }
    }

    /// <summary>
    /// Флаги свойств обработчика
    /// </summary>
    [Flags]
    public enum GetterType
    {
        None = 0,
        IsReadable = 1,
        IsWritable = 1 << 1,
        ReadParamRequired = 1 << 2,
        WriteParamRequired = 1 << 3
    }
    #endregion


    #region Target
    /// <summary>
    /// Объект обёртка для цели обработчика
    /// </summary>
    public interface ITarget
    {
        object GetTarget();
        TargetType TargetType { get; }
        string TargetName { get; }
        string TargetInfo { get; }
        IPropertySettings Settings { get; set; }
    }

    /// <summary>
    /// Типизированная обёртка цели
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITarget<out T> : ITarget
    {
        T Target { get; }
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
        Assembly = 1 << 2,
        Part = 1 << 3,
        Model = Assembly | Part,
        Drawing = 1 << 4
    } 
    #endregion


    /// <summary>
    /// Полученные данные
    /// </summary>
    public interface IComplexValue : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Доступ к дополнительным свойствам
        /// </summary>
        /// <param name="valueName"></param>
        /// <returns></returns>
        string this[string valueName] { get; set; }
        /// <summary>
        /// Имя свойства
        /// </summary>
        string PropertyName { get; }
        /// <summary>
        /// Значение для записи
        /// </summary>
        string BaseValue { get; set; }
    }

    /// <summary>
    /// Настройки дополнительных опций
    /// </summary>
    public interface IPropertySettings : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Получить свойство по ключу
        /// </summary>
        /// <param name="param">Имя свойства</param>
        /// <returns>В случае отсутствия возвращается null</returns>
        string this[string param] { get; }
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
}
