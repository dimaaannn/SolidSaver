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
            newBind.TargetRef = target;
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
            newBind.TargetRef = target;
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
    }



}
