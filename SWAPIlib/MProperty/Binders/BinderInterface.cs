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
        PropertyUpdate Update { get; set; }
        string TargetName { get; }
        /// <summary>
        /// Получить сущность привязки
        /// </summary>
        /// <returns></returns>
        IDataEntity GetTarget();
        /// <summary>
        /// Задать новую сущность привязки
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        bool SetTarget(IDataEntity dataEntity);
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
        public ModelBinder() { }
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
                Update?.Invoke();
            }
        }
        /// <summary>
        /// Имя свойства привязки
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// Имя модели
        /// </summary>
        public string TargetName => Target.FileName;

        public PropertyUpdate Update { get; set; }

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
        public IDataEntity GetTarget()
        {
            return Target;
        }

        /// <summary>
        /// Сменить модель привязки
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public bool SetTarget(IDataEntity dataEntity)
        {
            bool ret = false;
            if(dataEntity is IModelEntity modelEnt)
            {
                this.Target = modelEnt;
                configName = null;
                ret = true;
            }
            return ret;
        }
    }
}
