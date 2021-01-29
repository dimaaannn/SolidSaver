using System;
using System.Diagnostics;
using System.IO;
using SWAPIlib.MProperty;

namespace SWAPIlib.MProperty.BaseProp
{


    /// <summary>
    /// Абстрактный класс для привязки моделей SW
    /// </summary>
    /// <typeparam name="T">Наследуется от IAppModel</typeparam>
    public abstract class PropBindSWModel<T> : IPropBinding<T> where T : IModelEntity
    {
        /// <summary>
        /// Конструктор для клонирования
        /// </summary>
        protected PropBindSWModel() 
        {
            //FlushLocalData += FlushValuesHandler;
        }
        /// <summary>
        /// Конструктор абстрактного класса
        /// </summary>
        public PropBindSWModel(T target) : this()
        {
            TargetWrapper = target;
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
                //Очистить локальные значения
                FlushValuesHandler(this, EventArgs.Empty);
                //Отписаться от событий старой цели
                FlushLocalData -= FlushValuesHandler;

                //Оповестить об изменении цели
                TargetChanged?.Invoke(this, value);
                //Установить новую привязку
                targetWrapper = value;

                //Заново подписаться на событие
                FlushLocalData += FlushValuesHandler;

                //Сообщение
                string infoState = (targetWrapper != null) ? $"set {targetWrapper.Title}" : "error";
                Debug.WriteLine("ModelBindingBase = target {0}", infoState);
            }
        }

        public bool IsReadable { get; set; }
        public bool IsWritable { get; set; }

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
                    ret = TargetWrapper.TempConfigName;
                else
                    ret = configName;
                return ret;
            }
            set
            {
                configName = value;
                targetUpdated.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Имя объекта привязки
        /// </summary>
        public virtual string TargetName => Path.GetFileName(TargetWrapper.FileName);
        /// <summary>
        /// Имя свойства отображаемое пользователю
        /// </summary>
        public abstract string DisplayPropName { get; }
        /// <summary>
        /// Текстовое описание свойства, задаётся статически
        /// </summary>
        public abstract string BindingInfo { get; }

        /// <summary>
        /// Внутреннее событие обновления
        /// </summary>
        protected event EventHandler targetUpdated;
        /// <summary>
        /// Изменён объект привязки (второй аргумент - новый объект привязки)
        /// </summary>
        public event EventHandler<IDataEntity> TargetChanged;

        /// <summary>
        /// Вызывать событие обновления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CallUpdateHandler(object sender, EventArgs e)
        {
            targetUpdated?.Invoke(sender, e);
        }
        /// <summary>
        /// Очистить локальные переменные
        /// </summary>
        public virtual void FlushValuesHandler(object sender, EventArgs e)
        {
            configName = null;
        }
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        #region Прокси события
        /// <summary>
        /// Прокси события обновление цели
        /// </summary>
        public event EventHandler TargetUpdated
        {
            add
            {
                targetUpdated += value;
                if (TargetWrapper != null)
                    TargetWrapper.UpdateValuesEvent += value;
            }
            remove
            {
                targetUpdated -= value;
                if (TargetWrapper != null)
                    TargetWrapper.UpdateValuesEvent -= value;
            }
        }
        /// <summary>
        /// Прокси для привязки цели
        /// </summary>
        public event EventHandler WriteDataValue
        {
            add
            {
                if (TargetWrapper != null)
                    TargetWrapper.WriteDataEvent += value;
            }
            remove
            {
                if (TargetWrapper != null)
                    TargetWrapper.WriteDataEvent -= value;
            }
        }
        /// <summary>
        /// Очистить локальные переменные
        /// </summary>
        public event EventHandler FlushLocalData
        {
            add
            {
                if(TargetWrapper != null)
                    TargetWrapper.FlushDataEvent += value;
            }
            remove
            {
                if (TargetWrapper != null)
                    TargetWrapper.FlushDataEvent -= value;
            }
        }
        #endregion

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
    }


}
