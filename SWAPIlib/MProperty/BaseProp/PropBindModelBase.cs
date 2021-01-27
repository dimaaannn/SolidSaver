using System;
using System.Diagnostics;
using SWAPIlib.MProperty;

namespace SWAPIlib.MProperty.BaseProp
{


    /// <summary>
    /// Абстрактный класс для привязки моделей SW
    /// </summary>
    /// <typeparam name="T">Наследуется от IAppModel</typeparam>
    public abstract class PropBindSWModel<T> : IPropBinding<T> where T : IAppModel
    {
        protected PropBindSWModel() { }
        /// <summary>
        /// Конструктор абстрактного класса
        /// </summary>
        public PropBindSWModel(T target) : this()
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
                targetRef = value;
                TargetChanged?.Invoke(this, targetRef);

                string infoState = (targetRef != null) ? $"{targetRef.Title} set" : "error";
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
                string ret;
                if (string.IsNullOrEmpty(configName))
                {
                    ret = TargetRef.ActiveConfigName;
                }
                else
                    ret = configName;
                return ret;
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

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Текстовое описание свойства, задаётся статически
        /// </summary>
        public abstract string BindingInfo { get; }

        
    }


}
