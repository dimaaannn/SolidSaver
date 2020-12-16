using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{
    /// <summary>
    /// Объект свойства
    /// </summary>
    public interface ISwProperty: INotifyPropertyChanged
    {
        /// <summary>
        /// Ссылка на активную модель
        /// </summary>
        AppModel appModel { get; set; }
        /// <summary>
        /// Доступ для чтения
        /// </summary>
        bool IsReadable { get; }
        /// <summary>
        /// Доступ для записи
        /// </summary>
        bool IsWritable { get; }
        /// <summary>
        /// Применимо к типу текущей модели
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Имя свойства (пользовательское)
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// Запрашиваемый параметр
        /// </summary>
        string PropertyName { get; }
        /// <summary>
        /// Значение параметра
        /// </summary>
        string PropertyValue { get; set; }

        /// <summary>
        /// Имя конфигурации (опционально)
        /// </summary>
        string ConfigName { get; set; }
        /// <summary>
        /// Обновить значения
        /// </summary>
        void Update();
        string ReadValue();
        /// <summary>
        /// Записать изменённое значение
        /// </summary>
        /// <returns></returns>
        bool WriteValue();

        bool IsModifyed { get;}
    }

    public interface ISwProperty<T> : ISwProperty
    {
        T RawPropertyValue { get; set; }
        T ReadRawValue();
    }


    /// <summary>
    /// Расширенные свойства модели
    /// </summary>
    public interface IFileModelProp
    {
        bool IsRoot { get; }
        string WorkFolder { get; set; }
        string ModelFolder { get; }
        string GetProjectData { get; }  //TODO убрать Legacy features в отдельный класс
        string ProjectNumber { get; }
        string ProjectClient { get; }
        string ProjectName { get; }
        string ProjectType { get; }

        bool IsImported { get; }
        bool IsLibModel { get; }

        bool IsSheetPart { get; }
        bool IsHasDrawing { get; }

    }

    //TODO Переработать интерфейс деталей
    /// <summary>
    /// Интерфейс деталей с конфигурацией
    /// </summary>
    public interface IConfProperty
    {
        string ActiveConfigName { get; }
        IEnumerable<string> ConfigNames { get; }
        IDictionary<string, string> ConfigPropertyList { get; }
        string GetPropValue(string confName, string fieldName);
        bool SetPropValue(string confName, string fieldName, string fieldVal);
    }



    //public interface IPropertyManager : IEnumerable<ISwProperty>
    //{
    //    AppModel SwModel { get; }
    //    void UpdateAll();
    //    void WriteAll();
    //    void AddItem(ISwProperty PropGetter);
    //    bool RemoveItem(int Index);
    //}

}
