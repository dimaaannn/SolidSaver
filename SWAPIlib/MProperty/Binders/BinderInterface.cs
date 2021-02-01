using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.MProperty
{
    /// <summary>
    /// Объект привязки
    /// </summary>
    public interface IBinder : ICloneable
    {
        object GetTarget();
    }
    /// <summary>
    /// Типизированный объект привязки
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBinder<out T> : IBinder
        where T : IDataEntity
    {
        T Target { get; }
    }

    /// <summary>
    /// Привязка к моделям
    /// </summary>
    public interface IModelBinder : IBinder<IModelEntity>
    {
        string ConfigName { get; set; }
        string Info { get; set; } //Заглушка
    }

    /// <summary>
    /// Привязка к сущности модели
    /// </summary>
    public class ModelBinder : IModelBinder
    {
        private ModelBinder() { }
        /// <summary>
        /// Создать сущность модели
        /// </summary>
        /// <param name="modelEnt"></param>
        public ModelBinder(IModelEntity modelEnt) : this()
        {
            Target = modelEnt;
        }
        string configName;

        /// <summary>
        /// Ссылка на сущность
        /// </summary>
        public IModelEntity Target { get; private set; }
        /// <summary>
        /// Имя конфигурации
        /// </summary>
        public string ConfigName
        {
            get
            {

                if (string.IsNullOrEmpty(configName))
                    return Target.TargetWrapper.ActiveConfigName;
                return configName;
            }
            set
            {
                configName = value;
            }
        }
        /// <summary>
        /// Имя свойства привязки
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Клонировать объект с свойствами
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var NewBinder = new ModelBinder()
            {
                Target = Target,
                ConfigName = configName,
                Info = Info,
            };
            return NewBinder;
        }
        /// <summary>
        /// Вернуть объект привязки
        /// </summary>
        /// <returns></returns>
        public object GetTarget()
        {
            return Target;
        }
    }
}
