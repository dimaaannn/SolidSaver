using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.Controller;

namespace SWAPIlib.MProperty
{
    public delegate bool Validator<in T>(T bindObj);
    public delegate string GetProp<in T>(T bindObj);
    public delegate bool SetProp<in T>(T bindObj, string propValue);


    public interface IPropGetter<in T>
    {
        bool IsValid(T bindObj);
        Validator<T> Validator { get; }
        GetProp<T> Getter { get; }
        SetProp<T> Setter { get; }
    }

    public class PropGetter<T> : IPropGetter<T>
    {
        /// <summary>
        /// Проверка применимости свойства к объекту
        /// </summary>
        public Validator<T> Validator { get; protected set; } = (x) => true;
        /// <summary>
        /// Получить значение
        /// </summary>
        public GetProp<T> Getter { get; protected set; }
        /// <summary>
        /// Записать значение
        /// </summary>
        public SetProp<T> Setter { get; protected set; }
        /// <summary>
        /// Проверить применимость свойства к объекту
        /// </summary>
        /// <param name="bindObj"></param>
        /// <returns></returns>
        public bool IsValid(T bindObj) => Validator.Invoke(bindObj);

    }
}

namespace SWAPIlib.MProperty.Delegates
{
    public static class Component
    {
        
    }
}