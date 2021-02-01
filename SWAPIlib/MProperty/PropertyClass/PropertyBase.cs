using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.MProperty
{

    /// <summary>
    /// Базовый класс Агрегатор свойств
    /// </summary>
    /// <typeparam name="TPropGetter"></typeparam>
    /// <typeparam name="TBinder"></typeparam>
    /// <typeparam name="TDataEntity"></typeparam>
    public class PropertyBase<TPropGetter, TBinder, TDataEntity>
    : IProperty<TPropGetter, TBinder, TDataEntity>
    where TPropGetter : IPropGetter
    where TBinder : IBinder, new()
    where TDataEntity : IDataEntity
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public PropertyBase()
        {
            binder = new TBinder();

            viewData = new PropView();
            ViewData.Update = Update;
            ViewData.WriteValue = WriteValue;
        }

        protected IPropView viewData;
        protected TPropGetter propGetter;
        protected TBinder binder;

        /// <summary>
        /// Валидация полей свойства
        /// </summary>
        public bool IsValid => PropGetter != null && Entity != null;
        /// <summary>
        /// Изменён пользователем
        /// </summary>
        public bool IsModifyed => ViewData.IsModifyed;
        /// <summary>
        /// Имя свойства
        /// </summary>
        public string PropertyName => PropGetter.DisplayPropName;
        /// <summary>
        /// Имя объекта привязки
        /// </summary>
        public string TargetName => Binder.TargetName;
        /// <summary>
        /// Отображение свойства
        /// </summary>
        public IPropView ViewData => viewData;
        /// <summary>
        /// Временное значение свойства
        /// </summary>
        public string Value { get => ViewData.Value; set => ViewData.Value = value; }

        /// <summary>
        /// Привязка к объекту
        /// </summary>
        public TDataEntity Entity
        {
            get => (TDataEntity)Binder.GetTarget();
            set
            {
                Binder.SetTarget(value);
                Update();
            }
        }
        /// <summary>
        /// Обёртка свойств объекта
        /// </summary>
        public TBinder Binder => binder;
        /// <summary>
        /// Обработчик свойств
        /// </summary>
        public TPropGetter PropGetter
        {
            get => propGetter;
            set
            {
                propGetter = value;
                ViewData.IsReadable = PropGetter?.IsReadable == true;
                ViewData.IsReadable = PropGetter?.IsWritable == true;
                ViewData.ClearSaved();
                Update();
            }
        }

        /// <summary>
        /// Обновить значение
        /// </summary>
        public virtual void Update()
        {
            ViewData.ClearSaved();
            if (Entity != null && PropGetter?.IsReadable == true)
                ViewData.SetSavedVal(PropGetter.GetValue(Binder));
        }
        /// <summary>
        /// Записать значение
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool WriteValue(string val)
        {
            bool ret = false;
            if (Entity != null && PropGetter?.IsWritable == true)
                ret = PropGetter.SetValue(Binder, val);

            if (ret)
                ViewData.ClearSaved();

            return ret;
        }

        public virtual object Clone()
        {
            var ret = new PropertyBase<TPropGetter, TBinder, TDataEntity>()
            {
                binder = (TBinder)Binder?.Clone(),
                PropGetter = propGetter
            };
            return ret;
        }

    }
}
