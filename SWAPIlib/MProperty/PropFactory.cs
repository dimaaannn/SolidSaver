using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SWAPIlib.MProperty.Getters;

namespace SWAPIlib.MProperty
{


    public static class PropModelFactory
    {
        /// <summary>
        /// Создать пустой прототип свойства
        /// </summary>
        /// <returns></returns>
        public static PropertyModel CreatePrototype()
        {
            return new PropertyModel(); 
        }

        /// <summary>
        /// Создать прототип на основе свойства
        /// </summary>
        /// <param name="propGetter"></param>
        /// <returns></returns>
        public static PropertyModel CreatePrototype(IModelGetter propGetter)
        {
            PropertyModel ret = null;
            if(propGetter != null)
            {
                PropertyModel property = new PropertyModel();
                property.SetBinder(propGetter);
                ret = property;
            }
            return ret;
        }

        /// <summary>
        /// Создать прототип на основе сущности
        /// </summary>
        /// <param name="modelEntity"></param>
        /// <returns></returns>
        public static PropertyModel CreatePrototype(IModelEntity modelEntity)
        {
            PropertyModel ret = null;
            if (modelEntity != null)
            {
                ret = new PropertyModel(modelEntity);
            }
            return ret;
        }

        /// <summary>
        /// Создать полноценное свойство с привязками
        /// </summary>
        /// <param name="propGetter"></param>
        /// <param name="modelEntity"></param>
        /// <returns></returns>
        public static PropertyModel CreatePrototype(
            IModelGetter propGetter, IModelEntity modelEntity)
        {
            PropertyModel ret = null;
            if (propGetter != null && modelEntity != null)
            {
                PropertyModel property = new PropertyModel();
                property.SetBinder(propGetter);
                property.SetTarget(modelEntity);
                ret = property;
            }
            return ret;
        }

        /// <summary>
        /// Создать свойство по прототипу
        /// </summary>
        /// <param name="templateProperty">Прототип свойства</param>
        /// <param name="modelEntity">Новая привязка</param>
        /// <returns>В случае неудачи возвращается null</returns>
        public static TProp PropertyByTemplate<TProp>(
            TProp templateProperty, IDataEntity modelEntity)
            where TProp : IProperty
        {
            TProp ret = default;
            if((templateProperty?.GetGetter().Validator(modelEntity)) == true){
                var newProp = (TProp)templateProperty.Clone();

                newProp.SetTarget(modelEntity);
                ret = newProp;
            }
            return ret;
        }

        /// <summary>
        /// Создать свойство по прототипу
        /// </summary>
        /// <param name="templateProperty">Прототип свойства</param>
        /// <param name="modelEntity">Новая привязка</param>
        /// <returns>В случае неудачи возвращается null</returns>
        public static TProp PropertyByTemplate<TProp, TGetter>(
            TProp templateProperty, TGetter modelGetter)
            where TProp : IProperty
            where TGetter : IPropGetter
        {
            TProp ret = default;
            bool state = false;
            if ((modelGetter?.Validator(templateProperty.GetTarget())) == true)
            {
                var newProp = (TProp)templateProperty.Clone();

                state = newProp.SetTarget(modelGetter);
                ret = newProp;
            }
            return state ? ret : default;
        }

        #region Старая фабрика
        /// <summary>
        /// Создать свойство с привязкой
        /// </summary>
        /// <param name="binding">Привязка не копируется</param>
        /// <returns></returns>
        //public static IProperty AttachToEntity(IDataEntity entity)
        //{

        //    var propView = new PropView(binder: binding);
        //    return propView;
        //}

        /// <summary>
        /// Создать свойство по прототипу
        /// </summary>
        /// <param name="binding">Прототип</param>
        /// <returns></returns>
        //public static IPropView CreateByProto(IPropGetter binding)
        //{
        //    var newBind = (IPropGetter)binding.Clone();
        //    var propView = new PropView(binder: newBind);
        //    return propView;
        //}
        //    /// <summary>
        //    /// Создать свойство по прототипу
        //    /// </summary>
        //    /// <param name="binding">Прототип</param>
        //    /// <param name="configName">Имя конфигурации</param>
        //    /// <returns></returns>
        //    public static IPropView CreateByProto(IPropGetter binding, string configName)
        //    {
        //        var newBind = (IPropGetter) binding.Clone();
        //        newBind.ConfigName = configName;
        //        var propView = new PropView(binder: newBind);
        //        return propView;
        //    }
        //    /// <summary>
        //    /// Создать свойство по прототипу
        //    /// </summary>
        //    /// <typeparam name="T">Тип прототипа</typeparam>
        //    /// <param name="binding">Типизированный прототип</param>
        //    /// <param name="target">Новая ссылка привязки</param>
        //    /// <returns></returns>
        //    public static IPropView CreateByProto<T>(IPropGetter<T> binding, T target)
        //    {
        //        var newBind = (IPropGetter<T>)binding.Clone();
        //        newBind.ConfigName = null;
        //        newBind.TargetWrapper = target;
        //        var propView = new PropView(binder: newBind);
        //        return propView;
        //    }
        //    /// <summary>
        //    /// Создать свойство по прототипу
        //    /// </summary>
        //    /// <typeparam name="T">Тип прототипа</typeparam>
        //    /// <param name="binding">Типизированный прототип</param>
        //    /// <param name="target">Новая ссылка привязки</param>
        //    /// <param name="configName">Новое имя конфигурации</param>
        //    /// <returns></returns>
        //    public static IPropView CreateByProto<T>(IPropGetter<T> binding, T target, 
        //        string configName)
        //    {
        //        var newBind = (IPropGetter<T>)binding.Clone();
        //        newBind.TargetWrapper = target;
        //        newBind.ConfigName = configName;
        //        var propView = new PropView(binder: newBind);
        //        return propView;
        //    }

        //    /// <summary>
        //    /// Создать группу свойств по прототипу
        //    /// </summary>
        //    /// <typeparam name="T"></typeparam>
        //    /// <param name="bindPrototype">Прототип привязки</param>
        //    /// <param name="targets">Коллекция целей</param>
        //    /// <returns>Список свойств</returns>
        //    public static List<IPropView> CreateByProto<T>(
        //        IPropGetter<T> bindPrototype, IEnumerable<T> targets)
        //    {
        //        var validTargets = from target in targets
        //                       where bindPrototype.Validator(target)
        //                       select target;

        //        var propList = validTargets.Select(x => CreateByProto(bindPrototype, x));
        //        return propList.ToList();
        //    }
        //    /// <summary>
        //    /// Создать группу свойств для одного объекта
        //    /// </summary>
        //    /// <typeparam name="T"></typeparam>
        //    /// <param name="bindPrototypes">Список прототипов</param>
        //    /// <param name="target">Цель привязки</param>
        //    /// <returns></returns>
        //    public static List<IPropView> CreateByProto<T>(
        //        IEnumerable<IPropGetter<T>> bindPrototypes, T target)
        //    {
        //        var validPrototypes = from prototype in bindPrototypes
        //                           where prototype.Validator(target)
        //                           select prototype;

        //        var propList = validPrototypes.Select(x => CreateByProto(x, target));
        //        return propList.ToList();
        //    }
        //    /// <summary>
        //    /// Создать группу свойств для группы объектов
        //    /// </summary>
        //    /// <typeparam name="T"></typeparam>
        //    /// <param name="bindPrototypes">Список прототипов</param>
        //    /// <param name="targets">Список целей</param>
        //    /// <returns></returns>
        //    public static List<IPropView> CreateByProto<T>(
        //        IEnumerable<IPropGetter<T>> bindPrototypes, IEnumerable<T> targets)
        //    {
        //        List<IPropView> ret = new List<IPropView>();

        //        foreach(var prototype in bindPrototypes)
        //        {
        //            ret.AddRange(CreateByProto(prototype, targets));
        //        }
        //        return ret;
        //    }

    }

        ///// <summary>
        ///// Фабрика привязок свойств
        ///// </summary>
        //public static class BindingFactory
        //{
        //    /// <summary>
        //    /// Создать именованную привязку
        //    /// </summary>
        //    /// <param name="propertyName">Имя свойства модели</param>
        //    /// <param name="target">Модель привязки (опционально)</param>
        //    /// <param name="configName">Конфигурация модели (опционально)</param>
        //    /// <returns>Прототип именованной привязки</returns>
        //    public static IPropGetter<IModelEntity> NamedProperty(
        //        string propertyName, IModelEntity target = null, string configName = null)
        //    {
        //        return new PropModelNamedParamGetter()
        //        {
        //            PropertyName = propertyName,
        //            TargetWrapper = target,

        //            ConfigName = configName

        //        };
        //    }
        //}

        #endregion

    }
