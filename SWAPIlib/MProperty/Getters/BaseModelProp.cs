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
        public abstract bool Validator(IDataEntity dataEntity);
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

        public string GetValue(IBinder binder)
        {
            if (binder is T tbinder)
                return GetValue(tbinder);
            else
                throw new ArgumentException("PropModelGetterBase: Передана Некорректная привязка");
        }

        public bool SetValue(IBinder binder, string newValue)
        {
            if (binder is T tbinder)
                return SetValue(tbinder,newValue);
            else
                throw new ArgumentException("PropModelGetterBase: Передана Некорректная привязка");
        }

        public bool Validator(IBinder binderRef)
        {
            if (binderRef is T tbinder)
                return Validator(tbinder);
            else
                return false;
        }

    }


}
