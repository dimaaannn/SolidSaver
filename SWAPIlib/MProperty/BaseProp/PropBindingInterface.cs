using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.Controller;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib;

namespace SWAPIlib.MProperty
{

    /// <summary>
    /// Привязка к объекту для получения и задания свойств
    /// </summary>
    public interface IPropBinding : ICloneable
    {
        /// <summary>
        /// Информация привязки
        /// </summary>
        string PropName { get; }
        /// <summary>
        /// Имя объекта привязки
        /// </summary>
        string TargetName { get; }
        /// <summary>
        /// Имя конфигурации
        /// </summary>
        string ConfigName { get; set; }
        /// <summary>
        /// Инфо о привязке
        /// </summary>
        string BindingInfo { get; }

        /// <summary>
        /// Допускается чтение
        /// </summary>
        bool IsReadable { get; }
        /// <summary>
        /// Допускается запись
        /// </summary>
        bool IsWritable { get; }
        /// <summary>
        /// Прочитать значение переменной
        /// </summary>
        /// <returns></returns>
        string GetValue();
        /// <summary>
        /// Записать значение
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        bool SetValue(string newValue);
    }

    /// <summary>
    /// Типизированная привязка к объекту
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    public interface IPropBinding<T> : IPropBinding
    {
        EventHandler WrapperUpdated { get; set; }
        /// <summary>
        /// Проверка допустимости объекта
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        bool Validator(T targetRef);
        /// <summary>
        /// Ссылка на объект с свойством
        /// </summary>
        T TargetWrapper { get; set; }
        /// <summary>
        /// Изменена модель привязки
        /// </summary>
        event EventHandler<T> TargetChanged;
        
    }

}

