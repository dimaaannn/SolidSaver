﻿using System;
using System.Diagnostics;

namespace SWAPIlib.MProperty.BaseProp
{
    /// <summary>
    /// Абстрактный класс для привязки моделей SW
    /// </summary>
    /// <typeparam name="T">Наследуется от IAppModel</typeparam>
    public abstract class PropBindSWModel<T> : IPropBinding<T> where T : IAppModel
    {
        /// <summary>
        /// Конструктор абстрактного класса
        /// </summary>
        public PropBindSWModel(T target)
        {
            TargetRef = target;
        }

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
                if (Validator(value))
                {
                    targetRef = value;
                    TargetChanged?.Invoke(this, targetRef);
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
        /// <summary>
        /// Поле для отображения имени свойства
        /// </summary>
        public virtual string Title { get => title; set => title = value; }
        /// <summary>
        /// Имя конфигурации для запроса свойств
        /// </summary>
        public virtual string ConfigName
        {
            get
            {
                if (string.IsNullOrEmpty(configName))
                {
                    configName = TargetRef.ActiveConfigName;
                }
                return configName;
            }
            set => configName = value;
        }
        protected string configName;

        /// <summary>
        /// Проверка поддержки свойства моделью
        /// </summary>
        /// <param name="targetRef"></param>
        /// <returns></returns>
        public abstract bool Validator(T targetRef);
        /// <summary>
        /// Получить значение свойства
        /// </summary>
        /// <returns></returns>
        public abstract string GetValue();
        /// <summary>
        /// Задать значение свойства
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public abstract bool SetValue(string newValue);


        public event EventHandler<T> TargetChanged;
        /// <summary>
        /// для вызова события из дочерних классов
        /// </summary>
        /// <param name="target"></param>
        protected void RaiseTargetChanged(T target)
        {
            TargetChanged?.Invoke(this, target);
        }
        /// <summary>
        /// Текстовое описание свойства, задаётся статически
        /// </summary>
        public abstract string BindingInfo { get; }
    }


}
