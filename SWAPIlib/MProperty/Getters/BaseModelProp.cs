using System;
using System.Diagnostics;
using System.IO;

namespace SWAPIlib.MProperty.Getters
{


    /// <summary>
    /// Абстрактный класс для привязки моделей SW
    /// </summary>
    /// <typeparam name="T">Наследуется от IAppModel</typeparam>
    public abstract class PropModelGetterBase<T> : IPropGetter<T> where T : IBinder
    {
        protected PropModelGetterBase() { }

        public bool IsReadable { get; protected set; }
        public bool IsWritable { get; protected set; }
        /// <summary>
        /// Имя свойства отображаемое пользователю
        /// </summary>
        public abstract string DisplayPropName { get; }
        /// <summary>
        /// Текстовое описание свойства, задаётся статически
        /// </summary>
        public abstract string PropertyInfo { get; }

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
        public abstract string GetValue(T binder);
        /// <summary>
        /// Задать значение свойства
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public abstract bool SetValue(T binder, string newValue);
    }


}
