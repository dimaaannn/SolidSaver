using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.PropertyBase
{
    public abstract class PropertyBase : IProperty
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private bool isWritable = false;
        protected string info;
        protected string name;
        protected string value;
        protected string tempValue
        

        /// <summary>
        /// Возможность записи новых значений
        /// </summary>
        public bool IsWritable { get => isWritable; protected set => isWritable = value; }

        public abstract bool IsModifyed { get; }

        /// <summary>
        /// Имя свойства
        /// </summary>
        public string Name
        {
            get
            {
                if (info is null)
                {
                    Logger.Trace("Get property name");
                    name = GetName();
                    Logger.Trace("Property name: {name}", name);
                }
                return info;
            }
        }
        /// <summary>
        /// Пояснительная информация
        /// </summary>
        public string Info
        {
            get
            {
                if (info is null)
                {
                    Logger.Trace("Get property {PropertyName} info", Name);
                    info = GetInfo();
                    Logger.Trace("Property info: {info}", info);
                }
                return info;
            }
        }
        /// <summary>
        /// Значение свойства
        /// </summary>
        public string Value
        {
            get
            {
                Logger.Trace("Property {name} invoke Value property", name);
                if(value == null)
                {
                    Logger.Info("Property {name} invoke GetValue func", name);
                    value = GetValue();
                }
                if(tempValue != null)
                {
                    Logger.Info("Property {name} return user modifyed value: {tempvalue}, cashed: {value}", name, tempValue, value);
                    return tempValue;
                }
                else
                {
                    Logger.Info("Property {name} return cashed value: {value}", name, value);
                    return value;
                }
            }
        }

        protected abstract string GetName();
        protected abstract string GetInfo();
        protected abstract string GetValue();

        /// <summary>
        /// Очистить временное значение
        /// </summary>
        public virtual void ClearSaved()
        {
            Logger.Trace("Property {name} temp value cleared", name);
            tempValue = null;
        }

        public virtual bool Update()
        {
            Logger.Info("Property {name} invoke Update", name);
            return true;
        }

        /// <summary>
        /// Записать значение
        /// </summary>
        /// <returns></returns>
        public virtual bool WriteValue()
        {
            Logger.Info("Property {name} invoke WriteValue with old value: {value} tempValue: {tempValue},  ", name, value, tempValue);
            return true;
        }
    }

    //public abstract class PropertyBase : IProperty
    //{

    //    public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    //    public PropertyBase(ITargetInteraction target, IPropertyGetter2 propertyGetter)
    //    {
    //        Target = target;
    //        Getter = propertyGetter;
    //        if(Target.PropertyValue == null)
    //        {
    //            Getter.CheckTarget(target); //Create InteractionValue in target
    //            if(target.PropertyValue == null)
    //            {
    //                string errorMessage = "PropertyBase: Wrong target type";
    //                throw new AggregateException(errorMessage);
    //                //TODO add logger
    //            }
    //        }

    //    }

    //    public ITargetInteraction Target { get; protected set; }
    //    public IPropertyGetter2 Getter { get; protected set; }
    //    public IInteractionValue PropertyValue => Target.PropertyValue;

    //    public bool IsModifyed => PropertyValue.UserEdit != null;

    //    public string Value {
    //        get
    //        {

    //            if (PropertyValue.IsReadable == true)
    //                return PropertyValue.UserEdit ?? PropertyValue.Current;
    //            else
    //                return "Not avaliable";
    //        }
    //        set { 
    //            if (PropertyValue.IsWritable == true)
    //                PropertyValue.UserEdit = value;                
    //            }
    //    }
    //    /// <summary>
    //    /// Очистить пользовательский ввод
    //    /// </summary>
    //    public void ClearSaved()
    //    {
    //        if(PropertyValue != null)
    //            PropertyValue.UserEdit = null ;
    //    }

    //    public bool Update()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool WriteValue()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
