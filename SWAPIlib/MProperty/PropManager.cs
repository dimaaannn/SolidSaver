using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.MProperty
{
    public interface IPropManager :IEnumerable<IPropView>
    {
        void CreatePropViews();
        void WriteAllValues();
        void UpdateAllValues();
        void ClearValues();
        /// <summary>
        /// обновление списка свойств
        /// </summary>
        event EventHandler PropViewUpdated;
    }

    public interface IPropManager<out TargetType, BindType> :IPropManager
        where BindType : IPropBinding<BindType>
    {
        TargetType Target { get; }
        List<IPropView<BindType>> PropViews { get; }
        List<BindType> PropBindings { get; set; }

    }

    public abstract class PropManagerBase<TargetType, BindType>
        : IPropManager<TargetType, BindType>
        where BindType : IPropBinding<BindType>
    {
        protected PropManagerBase(List<BindType> propBindings = null)
        {
            PropBindings = propBindings;
        }

        private TargetType target;
        private List<BindType> propBindings;
        private List<IPropView<BindType>> propViews;

        public abstract event EventHandler PropViewUpdated;

        /// <summary>
        /// Цель привязки
        /// </summary>
        public TargetType Target { get => target; 
            set
            {
                target = value;
                //Replace all target in bindings
                //update events
            } 
        }
        /// <summary>
        /// Список привязок для представлений
        /// </summary>
        public List<BindType> PropBindings { get => propBindings; set => propBindings = value; }
        /// <summary>
        /// Список представлений
        /// </summary>
        public List<IPropView<BindType>> PropViews => propViews;

        /// <summary>
        /// Очистить значения представлений
        /// </summary>
        public void ClearValues()
        {
            PropViews.ForEach(x => x.ClearValues());
        }
        /// <summary>
        /// Обновить значения представлений
        /// </summary>
        public void UpdateAllValues()
        {
            PropViews.ForEach(x => x.Update());
        }
        /// <summary>
        /// Записать значения представлений
        /// </summary>
        public void WriteAllValues()
        {
            PropViews.ForEach(x => x.WriteValue());
        }

        /// <summary>
        /// Сформировать список представлений
        /// </summary>
        public abstract void CreatePropViews();


        /// <summary>
        /// Преобразование типа IEnumerator
        /// </summary>
        /// <typeparam name="T">Тип преобразования</typeparam>
        /// <param name="iterator">Targer IEnumerator</param>
        /// <returns></returns>
        protected static IEnumerator<T> Cast<T>(IEnumerator iterator)
        {
            while (iterator.MoveNext())
            {
                yield return (T)iterator.Current;
            }
        }
        public virtual IEnumerator<IPropView> GetEnumerator()
        {
            return Cast<IPropView>(PropViews.GetEnumerator());
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return PropViews.GetEnumerator();
        }
    }

}
