using System;
using System.Diagnostics;
using SWAPIlib.MProperty;

namespace SWAPIlib.MProperty.BaseProp
{


    /// <summary>
    /// Абстрактный класс для привязки моделей SW
    /// </summary>
    /// <typeparam name="T">Наследуется от IAppModel</typeparam>
    public abstract class PropBindSWModel<T> : IPropBinding<T> where T : IModelFields
    {
        protected PropBindSWModel() { }
        /// <summary>
        /// Конструктор абстрактного класса
        /// </summary>
        public PropBindSWModel(T target) : this()
        {
            TargetWrapper = target;
            //Событие изменения свойств
            WrapperUpdated = (x, y) =>
            {
                this.TargetChanged?.Invoke(this, (T)x);
            };
        }

        
        /// <summary>
        /// Основная привязка
        /// </summary>
        protected T targetWrapper;
        public T TargetWrapper
        {
            get => targetWrapper;
            set
            {
                targetWrapper = value;

                string infoState = (targetWrapper != null) ? $"set {targetWrapper.Title}" : "error";
                Debug.WriteLine("ModelBindingBase = target {0}", infoState);
            }
        }

        public bool IsReadable { get; set; }
        public bool IsWritable { get; set; }

        public abstract string PropName { get; }
        protected string configName;
        /// <summary>
        /// Имя конфигурации для запроса свойств
        /// </summary>
        public string ConfigName
        {
            get
            {
                string ret;
                if (string.IsNullOrEmpty(configName))
                    ret = TargetWrapper.SelectedConfigName;
                else
                    ret = configName;
                return ret;
            }
            set => configName = value;
        }

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
        /// <summary>
        /// Имя объекта привязки
        /// </summary>
        public virtual string TargetName => TargetWrapper.FileName;

        public EventHandler WrapperUpdated { get; set; }
    }


}
