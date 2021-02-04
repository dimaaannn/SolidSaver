using SWAPIlib.MProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.PropReplacer
{
    /// <summary>
    /// Генератор свойств
    /// </summary>
    public interface IPropMaker
    {
        /// <summary>
        /// Сгенерировать список свойств
        /// </summary>
        /// <returns></returns>
        IProperty[] GetProperties();

        /// <summary>
        /// Добавить привязку
        /// </summary>
        /// <param name="propGetter"></param>
        void AddTarget(IPropGetter propGetter);
        /// <summary>
        /// Добавить сущность
        /// </summary>
        /// <param name="entity"></param>
        void AddTarget(IDataEntity entity);

        /// <summary>
        /// очистить список сущностей
        /// </summary>
        void ClearEntityList();
        /// <summary>
        /// Очистить список привязок
        /// </summary>
        void ClearGettersList();
    }

    /// <summary>
    /// Генератор свойств для модели
    /// </summary>
    public interface IModelPropMaker : IPropMaker
    {
        /// <summary>
        /// Создать свойство для каждой конфигурации
        /// </summary>
        bool ForEachConfig { get; set; }
        /// <summary>
        /// Фильтр для имён конфигураций
        /// </summary>
        Func<string, bool> ConfigNameFilter { get; set; }

        /// <summary>
        /// Список свойств
        /// </summary>
        List<IModelGetter> Getters { get; }
        /// <summary>
        /// Список сущностей
        /// </summary>
        List<IModelEntity> DataEntities { get; }

        List<IPropertyModel> GetModelProperties();
    }


    public class ModelPropMaker : IModelPropMaker
    {
        public ModelPropMaker()
        {
            DataEntities = new List<IModelEntity>();
            Getters = new List<IModelGetter>();
        }

        /// <summary>
        /// Создать свойство для каждой конфигурации
        /// </summary>
        public bool ForEachConfig { get; set; } = false; //По умолчанию отключено
        /// <summary>
        /// Делегат фильтр для имени конфигурации
        /// </summary>
        public Func<string, bool> ConfigNameFilter { get; set; }

        /// <summary>
        /// Список сущностей
        /// </summary>
        public List<IModelEntity> DataEntities { get; set; }
        /// <summary>
        /// Список привязок
        /// </summary>
        public List<IModelGetter> Getters { get; set; }

        /// <summary>
        /// Сгенерировать список свойств
        /// </summary>
        public List<IPropertyModel> GetModelProperties()
        {
            return PropModelFactory.GeneratePropertySet(
                DataEntities, Getters, ForEachConfig, ConfigNameFilter);
        }
        IProperty[] IPropMaker.GetProperties() => GetModelProperties().ToArray();

        public void AddTarget(IPropGetter propGetter)
        {
            if (propGetter is IModelGetter mgetter)
                Getters.Add(mgetter);
        }
        public void AddTarget(IModelGetter propGetter) => Getters.Add(propGetter);
        public void AddTarget(IDataEntity entity)
        {
            if (entity is IModelEntity modelEnt)
                AddTarget(modelEnt);
        }
        public void AddTarget(IModelEntity modelEntity) => DataEntities.Add(modelEntity);

        public void ClearEntityList() => DataEntities.Clear();
        public void ClearGettersList() => Getters.Clear();


    }
}
