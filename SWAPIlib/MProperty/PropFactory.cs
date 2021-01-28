using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.MProperty.BaseProp;

namespace SWAPIlib.MProperty
{
    public static class PropFactory
    {
        /// <summary>
        /// Создать свойство с привязкой
        /// </summary>
        /// <param name="binding">Привязка не копируется</param>
        /// <returns></returns>
        public static IPropView AttachToBinding(IPropBinding binding)
        {
            var propView = new PropViewB(binder: binding);
            return propView;
        }

        /// <summary>
        /// Создать свойство по прототипу
        /// </summary>
        /// <param name="binding">Прототип</param>
        /// <param name="configName">Имя конфигурации</param>
        /// <returns></returns>
        public static IPropView CreateByProto(IPropBinding binding, string configName)
        {
            var newBind = (IPropBinding) binding.Clone();
            newBind.ConfigName = configName;
            var propView = new PropViewB(binder: newBind);
            return propView;
        }
        /// <summary>
        /// Создать свойство по прототипу
        /// </summary>
        /// <typeparam name="T">Тип прототипа</typeparam>
        /// <param name="binding">Типизированный прототип</param>
        /// <param name="target">Новая ссылка привязки</param>
        /// <returns></returns>
        public static IPropView CreateByProto<T>(IPropBinding<T> binding, T target)
        {
            var newBind = (IPropBinding<T>)binding.Clone();
            newBind.ConfigName = null;
            newBind.TargetWrapper = target;
            var propView = new PropViewB(binder: newBind);
            return propView;
        }
        /// <summary>
        /// Создать свойство по прототипу
        /// </summary>
        /// <typeparam name="T">Тип прототипа</typeparam>
        /// <param name="binding">Типизированный прототип</param>
        /// <param name="target">Новая ссылка привязки</param>
        /// <param name="configName">Новое имя конфигурации</param>
        /// <returns></returns>
        public static IPropView CreateByProto<T>(IPropBinding<T> binding, T target, 
            string configName)
        {
            var newBind = (IPropBinding<T>)binding.Clone();
            newBind.TargetWrapper = target;
            newBind.ConfigName = configName;
            var propView = new PropViewB(binder: newBind);
            return propView;
        }

        /// <summary>
        /// Создать группу свойств по прототипу
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindPrototype">Прототип привязки</param>
        /// <param name="targets">Коллекция целей</param>
        /// <returns>Список свойств</returns>
        public static List<IPropView> CreateByProto<T>(
            IPropBinding<T> bindPrototype, IEnumerable<T> targets)
        {
            var validTargets = from target in targets
                           where bindPrototype.Validator(target)
                           select target;

            var propList = validTargets.Select(x => CreateByProto(bindPrototype, x));
            return propList.ToList();
        }
        /// <summary>
        /// Создать группу свойств для одного объекта
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindPrototypes">Список прототипов</param>
        /// <param name="target">Цель привязки</param>
        /// <returns></returns>
        public static List<IPropView> CreateByProto<T>(
            IEnumerable<IPropBinding<T>> bindPrototypes, T target)
        {
            var validPrototypes = from prototype in bindPrototypes
                               where prototype.Validator(target)
                               select prototype;

            var propList = validPrototypes.Select(x => CreateByProto(x, target));
            return propList.ToList();
        }
        /// <summary>
        /// Создать группу свойств для группы объектов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindPrototypes">Список прототипов</param>
        /// <param name="targets">Список целей</param>
        /// <returns></returns>
        public static List<IPropView> CreateByProto<T>(
            IEnumerable<IPropBinding<T>> bindPrototypes, IEnumerable<T> targets)
        {
            List<IPropView> ret = new List<IPropView>();

            foreach(var prototype in bindPrototypes)
            {
                ret.AddRange(CreateByProto(prototype, targets));
            }
            return ret;
        }

    }

    /// <summary>
    /// Фабрика привязок свойств
    /// </summary>
    public static class BindingFactory
    {
        /// <summary>
        /// Создать именованную привязку
        /// </summary>
        /// <param name="propertyName">Имя свойства модели</param>
        /// <param name="target">Модель привязки (опционально)</param>
        /// <param name="configName">Конфигурация модели (опционально)</param>
        /// <returns>Прототип именованной привязки</returns>
        public static IPropBinding<IModelFields> NamedProperty(
            string propertyName, IModelFields target = null, string configName = null)
        {
            return new PropBindNamed()
            {
                PropertyName = propertyName,
                TargetWrapper = target,
                
                ConfigName = configName
                
            };
        }
    }


}
