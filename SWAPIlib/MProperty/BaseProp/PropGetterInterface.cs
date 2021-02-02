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
    public interface IPropGetter 
    {
        /// <summary>
        /// Информация привязки
        /// </summary>
        string DisplayPropName { get; }
        /// <summary>
        /// Допускается чтение
        /// </summary>
        bool IsReadable { get; }
        /// <summary>
        /// Допускается запись
        /// </summary>
        bool IsWritable { get; }
        /// <summary>
        /// Текстовое описание свойства, задаётся статически
        /// </summary>
        string PropertyInfo { get; }
        /// <summary>
        /// Прочитать значение переменной
        /// </summary>
        /// <returns></returns>
        string GetValue(IBinder binder);
        /// <summary>
        /// Записать значение
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        bool SetValue(IBinder binder, string newValue);
        /// <summary>
        /// Проверка допустимости объекта
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        bool Validator(IBinder binderRef);
    }

    /// <summary>
    /// Типизированное свойство объекта
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    public interface IPropGetter<T> : IPropGetter
        where T : IBinder
    {
        /// <summary>
        /// Прочитать значение переменной
        /// </summary>
        /// <returns></returns>
        string GetValue(T binder);
        /// <summary>
        /// Записать значение
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        bool SetValue(T binder, string newValue);
        /// <summary>
        /// Проверка допустимости объекта
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        bool Validator(T binderRef);
        bool Validator(IDataEntity dataEntity);
    }

    public interface IModelGetter : IPropGetter<IModelBinder>
    {

    }

    //Переместить в отдельный файл



}

