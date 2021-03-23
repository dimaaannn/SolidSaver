using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.MProperty.Getters;

namespace SWAPIlib.MProperty
{

    public class ModelPropertyBuilder
    {
        private IModelBinder modelBinder;
        private IModelEntity modelEntity;
        private IPropGetter<IModelBinder> propGetter;

        /// <summary>
        /// Создать пустое свойство
        /// </summary>
        /// <returns></returns>
        public IPropertyModel Create()
        {
            var tempProp = new PropertyModel() {Binder = (ModelBinder)modelBinder;
            
        }


    }

    public static class PropFactory
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
        public static TProp CreatePrototype<TProp>(IPropGetter propGetter)
            where TProp : IProperty, new()
        {
            TProp ret = default;
            if(propGetter != null)
            {
                TProp property = new TProp();
                property.SetTarget(propGetter);
                ret = property;
            }
            return ret;
        }

        /// <summary>
        /// Создать прототип на основе сущности
        /// </summary>
        /// <typeparam name="TProp">Тип свойства</typeparam>
        /// <param name="dataEntity">Сущность</param>
        /// <returns></returns>
        public static TProp CreatePrototype<TProp>(IDataEntity dataEntity)
            where TProp : IProperty, new()
        {
            TProp ret = default;
            if (dataEntity != null)
            {
                ret = new TProp();
                ret.SetTarget(dataEntity);
            }
            return ret;
        }

        /// <summary>
        /// Создать полноценное свойство с привязками
        /// </summary>
        /// <param name="propGetter"></param>
        /// <param name="modelEntity"></param>
        /// <returns></returns>
        public static TProp CreatePrototype<TProp>(
            IPropGetter propGetter, IDataEntity modelEntity)
            where TProp : IProperty, new()
        {
            TProp ret = default;
            if (propGetter != null && modelEntity != null)
            {
                var property = new TProp();
                property.SetTarget(propGetter);
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
        public static TProp PropertyByTemplate<TProp>(
            TProp templateProperty, IPropGetter modelGetter)
            where TProp : IProperty
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

        /// <summary>
        /// свойства на основе списка обработчиков
        /// </summary>
        /// <typeparam name="TProp">тип свойства</typeparam>
        /// <typeparam name="TGetter">Тип обработчика</typeparam>
        /// <param name="templateProperty">Свойство с привязкой к сущности</param>
        /// <param name="modelGetters">Список обработчиков</param>
        /// <returns></returns>
        public static  List<TProp> PropertyByList<TProp>(
            TProp templateProperty, IEnumerable<IPropGetter> modelGetters)
            where TProp : IProperty
        {
            if (templateProperty.GetTarget() == null)
                throw new ArgumentNullException("PropertyByTemplate: Отсутствует сущность");

            var newProps = modelGetters.Select(
                getter => PropertyByTemplate(templateProperty, getter));

            return newProps.Where(x => x != null).ToList();
        }


        /// <summary>
        /// свойства на основе списка сущностей
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="templateProperty">Свойство с обработчиком</param>
        /// <param name="entityList">Список сущностей</param>
        /// <returns></returns>
        public static List<TProp> PropertyByList<TProp>(
            TProp templateProperty, IEnumerable<IDataEntity> entityList)
            where TProp : IProperty
        {
            if (templateProperty.GetGetter() == null)
                throw new ArgumentNullException("PropertyByTemplate: Отсутствует обработчик");
            var newProps = entityList.Select(
                entity => PropertyByTemplate(templateProperty, entity));

            return newProps.Where(x => x != null).ToList();
        }

        ///// <summary>
        ///// Свойства по списку привязок и сущностей
        ///// </summary>
        ///// <typeparam name="TProp"></typeparam>
        ///// <param name="templateProperty">Шаблон свойства</param>
        ///// <param name="modelGetters">Список привязок</param>
        ///// <param name="dataEntities">Список сущностей</param>
        ///// <returns></returns>
        //public static List<TProp> PropertyByTemplate<TProp>(
        //    TProp templateProperty, 
        //    IEnumerable<IPropGetter> modelGetters, 
        //    IEnumerable<IDataEntity> dataEntities)

        //    where TProp : IProperty, new()
        //{
        //    var ret = new List<TProp>();
        //    var propWithGetters = modelGetters.Select(
        //        getter => CreatePrototype<TProp>(getter));

        //    foreach(var property in propWithGetters)
        //    {
        //        ret.AddRange(PropertyByTemplate(property, dataEntities));
        //    }

        //    return ret;
        //}


        public static List<TProp> PropertyByTemplate<TProp>(
            IEnumerable<TProp> propWithEntity,
            IEnumerable<IPropGetter> getters)

            where TProp : IProperty, new()
        {
            var ret = new List<TProp>();
            foreach (var propTemplate in propWithEntity)
            {
                ret.AddRange(
                    getters.Select(
                        getter => PropertyByTemplate(propTemplate, getter)));
            }

            return ret;
        }

        /// <summary>
        /// Прикрепить привязку к списку свойств
        /// </summary>
        /// <param name="templateProperties"></param>
        /// <param name="modelGetter"></param>
        public static void AttachToTemplate(
            IEnumerable<IProperty> templateProperties, 
            IPropGetter modelGetter)
        {
            foreach(var prop in templateProperties)
            {
                prop.SetTarget(modelGetter);
            }
        }


        /// <summary>
        /// Прикрепить сущность к списку свойств 
        /// </summary>
        /// <param name="templateProperties"></param>
        /// <param name="entity"></param>
        public static void AttachToTemplate(
            IEnumerable<IProperty> templateProperties,
            IDataEntity entity)
        {
            foreach (var prop in templateProperties)
            {
                prop.SetTarget(entity);
            }
        }


    }

    public static class PropModelFactory
    {
        //TODO фильтровать развёртки
        /// <summary>
        /// Фильтр по умолчанию для имён конфигураций
        /// </summary>
        /// <param name="confName">Имя конфигурации</param>
        /// <returns></returns>
        public static bool ConfigNamesFilter(string confName)
        {
            return true;
        }

        /// <summary>
        /// Создать пустой прототип
        /// </summary>
        /// <returns></returns>
        public static IPropertyModel CreatePrototype()
        {
            return new PropertyModel();
        }
        /// <summary>
        /// Создать прототип с привязкой к модели
        /// </summary>
        /// <param name="modelEntity"></param>
        /// <returns></returns>
        public static IPropertyModel CreatePrototype(IModelEntity modelEntity)
        {
            return new PropertyModel() { Entity = modelEntity };

        }

        /// <summary>
        /// Создать прототип с именем конфигурации
        /// </summary>
        /// <param name="modelEntity"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static IPropertyModel CreatePrototype(
            IModelEntity modelEntity, string configName)
        {
            var ret = new PropertyModel() { Entity = modelEntity };
            ret.Binder.ConfigName = configName;
            return ret;
        }



        /// <summary>
        /// Создать список прототипов для конфигураций
        /// </summary>
        /// <param name="modelEntity">Сущность модели</param>
        /// <param name="modelGetter">Обработчик свойств</param>
        /// <param name="forAllCongigs">Для всех конфигураций</param>
        /// <returns></returns>
        public static List<IPropertyModel> CreatePrototypeSet(
            IModelEntity modelEntity, 
            bool forAllCongigs, 
            Func<string, bool> confNameFilter = null)
        {
            List<IPropertyModel> ret = new List<IPropertyModel>() ;
            //Фильтр по умолчанию
            confNameFilter = confNameFilter ?? ConfigNamesFilter;

            if (forAllCongigs)
            {
                var filteredConfigNames = modelEntity.ConfigNames.Where(
                    ConfigNamesFilter);
                ret.AddRange(filteredConfigNames.Select(
                    confname => CreatePrototype(modelEntity, confname)));
            }
            else
                ret.Add(
                    CreatePrototype(modelEntity));

            return ret;
        }

        /// <summary>
        /// Создать прототипы для набора сущностей с конфигурациями
        /// </summary>
        /// <param name="modelEntity"></param>
        /// <param name="forAllCongigs"></param>
        /// <returns></returns>
        public static List<IPropertyModel> CreatePrototypeSet(
            IEnumerable<IModelEntity> modelEntity, 
            bool forAllCongigs,
            Func<string, bool> confNameFilter = null)
        {
            List<IPropertyModel> ret = new List<IPropertyModel>();
            var test = modelEntity.Select(entity => CreatePrototypeSet(
                    entity, forAllCongigs, confNameFilter));

            //Group to 1 list
            ret.AddRange(test.SelectMany(x => x));

            return ret;
        }

        /// <summary>
        /// Глобальная фабрика для списка свойств и привязок модели
        /// </summary>
        /// <param name="entities">Список сущностей</param>
        /// <param name="modelGetters">Список привязок</param>
        /// <param name="forAllConfigs">Генерировать для всех конфигураций</param>
        /// <param name="confNameFilter">Фильтр имён конфигураций</param>
        /// <returns></returns>
        public static List<IPropertyModel> GeneratePropertySet(
            IEnumerable<IModelEntity> entities, 
            IEnumerable<IModelGetter> modelGetters,
            bool forAllConfigs, 
            Func<string, bool> confNameFilter = null)
        {
            //var ret = new List<IPropertyModel>();

            var allConfigProps = PropModelFactory.CreatePrototypeSet(
                entities, true, confNameFilter);

            var resultProperty = allConfigProps.Select(propTempl =>
                PropFactory.PropertyByList(propTempl, modelGetters)).SelectMany(x => x);

            //ret.AddRange(resultProperty);
            return resultProperty.ToList();
        }


        public static List<IPropertyModel> DefaultModel(IModelEntity modelEntity)
        {
            var ret = new List<IPropertyModel>();
            var appmodel = modelEntity?.TargetWrapper;

            if(appmodel != null)
            {
                var propTemplate = CreatePrototype(modelEntity);

                var getters = new IModelGetter[]
                {
                    new PropModelNamedParamGetter("Обозначение"),
                    new PropModelNamedParamGetter("Наименование"),
                    //Дополнить
                };

                ret.AddRange(PropFactory.PropertyByList(propTemplate, getters));

                //switch (appmodel)
                //{
                //    case ISwPart p:


                //    default:
                //        break;
                //}

            }
            return ret;
        }
    }

       
}
