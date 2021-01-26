using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.Controller;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib;
using System.Diagnostics;

namespace SWAPIlib.MProperty
{
    public delegate bool Validator<in T>(T Target);
    public delegate string GetProp<in T>(T Target);
    public delegate bool SetProp<in T>(T Target, string propValue);

    public class GetterSettings
    {

    }

    public interface IPropertyTarget
    {
        string Title { get; }
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

    public interface IPropertyTarget<T> : IPropertyTarget
    {
        T TargetRef { set; }
        bool Validator(T targetRef);
    }

    public interface IPropGetter<in T>
    {
        bool IsWritable { get; }
        bool IsReadable { get; }
        /// <summary>
        /// Валидность объекта
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool IsValid(T target);
        /// <summary>
        /// Получить значение свойства
        /// </summary>
        /// <returns></returns>
        string GetValue(T target);
        /// <summary>
        /// Задать значение свойства
        /// </summary>
        /// <returns></returns>
        bool SetValue(T target, string value);
    }

    public interface IPropModelNamed : IPropGetter<IAppModel>
    {
        bool SetValue(IAppModel target, string value, 
            string propName, string configName);
        string GetValue(IAppModel target, string propertyName, 
            string configName);
    }


    /// <summary>
    /// Именованные свойства модели (обозначение, и пр.)
    /// </summary>
    public class PropModelNamed : IPropModelNamed
    {
        public bool IsReadable => true;
        public bool IsWritable => true;

        public bool IsValid(IAppModel target)
        {
            bool ret = false;
            if (
                target.DocType == (AppDocType.swPART | AppDocType.swASM) &&
                target.SwModel != null)
            {
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Получить именованное свойство
        /// </summary>
        /// <param name="target">Модель</param>
        /// <param name="propertyName">Имя свойства из модели</param>
        /// <param name="configName">Имя конфигурации, или активная конфигурация</param>
        /// <returns>Значение свойства (не вычисленное)</returns>
        public string GetValue(IAppModel target, string propertyName, string configName = null)
        {
            string ret = null;
            if(IsValid(target) && string.IsNullOrEmpty(propertyName))
            {
                var model = target.SwModel;

                //Имя конфигурации по умолчанию
                if (string.IsNullOrEmpty(configName))
                    configName = ModelConfigProxy.GetActiveConfName(model);

                ret = ModelConfigProxy.GetConfParam(model, configName, propertyName);
            }
            Debug.WriteLine($"ReadValue PropModelNamed {target.Title} = {ret}");
            return ret;
        }

        /// <summary>
        /// Установить значение именованного свойства
        /// </summary>
        /// <param name="target">Модель</param>
        /// <param name="value">Новое значение свойства</param>
        /// <param name="propName">Имя свойства из модели</param>
        /// <param name="configName">Имя конфигурации или активная конф.</param>
        /// <returns></returns>
        public bool SetValue(IAppModel target, string value, string propName, string configName)
        {
            var ret = false;
            Debug.WriteLine($"Write val AppModelPropGetter {target.Title} - {value}");
            if (IsValid(target) && string.IsNullOrEmpty(propName))
            {
                var model = target.SwModel;

                //Имя конфигурации по умолчанию
                if (string.IsNullOrEmpty(configName))
                    configName = ModelConfigProxy.GetActiveConfName(model);

                ret = ModelConfigProxy.SetConfParam(model, propName, value, configName);
            }

            string WriteStatus = ret ? "written" : "Not written";
            Debug.Write($"Value was {WriteStatus}");
            return ret;
        }

        /// <summary>
        /// Заглушка
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(IAppModel target, string value)
        {
            throw new ArgumentException("Не указано имя именованного свойства детали");
        }
        /// <summary>
        /// Заглушка
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public string GetValue(IAppModel target)
        {
            throw new ArgumentException("Не указано имя именованного свойства детали");
        }
    }
}

//namespace SWAPIlib.MProperty.Templates
//{
    
//}