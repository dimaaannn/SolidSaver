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

    public interface IPropertyBinding
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
        /// <summary>
        /// Инфо о привязке
        /// </summary>
        string BindingInfo { get; }
    }

    public interface IModelBinding<T> : IPropertyBinding
    {
        /// <summary>
        /// Проверка допустимости объекта
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        bool Validator(T targetRef);
        /// <summary>
        /// Ссылка на объект с свойством
        /// </summary>
        T TargetRef { get; }
        /// <summary>
        /// Имя конфигурации модели
        /// </summary>
        string ConfigName { get; set; }
        /// <summary>
        /// Изменена модель привязки
        /// </summary>
        event EventHandler<T> TargetChanged;
        
    }


    public abstract class BindModelBaseProp<T> : IModelBinding<T> where T : IAppModel
    {
        /// <summary>
        /// Основная привязка
        /// </summary>
        protected T targetRef;
        public T TargetRef 
        { 
            get => targetRef; 
            set
            {
                bool status = false;
                if (!Validator(targetRef))
                {
                    targetRef = value;
                    TargetChanged.Invoke(this, targetRef);
                    status = true;
                }
                else
                {
                    IsReadable = false;
                    IsWritable = false;
                }
                string infoState = status ? $"{targetRef.Title} set" : "error";
                Debug.WriteLine("ModelBindingBase = target {0}", infoState);
            }
        }

        public bool IsReadable { get; set; }
        public bool IsWritable { get; set; }

        protected string title;
        public virtual string Title { get => title; set => title = value; }
        public virtual string ConfigName { get; set; }

        public abstract bool Validator(T targetRef);
        public abstract string GetValue();
        public abstract bool SetValue(string newValue);

        public event EventHandler<T> TargetChanged;
        public string BindingInfo { get; set; }
    }

    public class BindModelNamedProp : BindModelBaseProp<IAppModel>
    {
        private string propertyName;

        BindModelNamedProp(string propertyName)
        {
            IsReadable = true;
            IsWritable = true;
            PropertyName = propertyName;
        }

        public string PropertyName 
        { 
            //Свойство по умолчанию
            get => propertyName ?? "Наименование"; 
            set => propertyName = value; 
        }

        /// <summary>
        /// Свойство конфигурации
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public string GetValue(string configName)
        {
            return TargetRef[configName, PropertyName];
        }
        /// <summary>
        /// Свойство активной конфигурации
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            return GetValue(TargetRef.ActiveConfigName);
        }

        /// <summary>
        /// Задать значение свойства
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool SetValue(string configName, string newValue)
        {
            return TargetRef.SetParameterVal(configName: configName, paramName: PropertyName, newValue: newValue);
        }
        /// <summary>
        /// Свойство в активной конфигурации
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public override bool SetValue(string newValue)
        {
            return SetValue(TargetRef.ActiveConfigName, newValue: newValue);
        }
        /// <summary>
        /// Проверить наличие конкретного свойства в модели
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        public override bool Validator(IAppModel targetRef)
        {
            bool ret = true;

            if(targetRef != null)
                ret = targetRef.ParameterList.Contains(PropertyName);

            return ret;
        }
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