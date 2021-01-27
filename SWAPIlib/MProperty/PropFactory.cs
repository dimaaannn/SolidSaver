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

    }
}
