using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    /// <summary>
    /// Обёртка для свойств объекта
    /// </summary>
    /// <typeparam name="TWrapper"></typeparam>
    /// <typeparam name="TBinder"></typeparam>
    public interface IPropManager<out TWrapper, TPropView, TBinder> 
        : IPropManager
        where TWrapper : IDataEntity
        where TPropView : IPropView
        where TBinder : IPropGetter
    {
        TWrapper TargetWrapper { get; }
        /// <summary>
        /// Представления свойств
        /// </summary>
        ObservableCollection<TPropView> PropViews { get; }
        /// <summary>
        /// Прототипы свойств для создания представлений
        /// </summary>
        List<TBinder> PropBindings { get; set; }
    }



    #region PropManagerBase

    ///// <summary>
    ///// Осуществляет управление представлениями свойств для конкретного объекта
    ///// </summary>
    ///// <typeparam name="TWrapper">Тип объекта</typeparam>
    ///// <typeparam name="TBind">Тип привязки</typeparam>
    //public abstract class PropManagerBase<TWrapper, TPropView, TBind>
    //    : IPropManager<TWrapper, TPropView, TBind>
    //    where TWrapper : IDataEntity
    //    where TPropView : IPropView
    //    where TBind : IPropBinding
    //{
    //    protected PropManagerBase() 
    //    {
    //        propViews = new ObservableCollection<TPropView>();
    //    }

    //    //private void PropViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    //    //{
    //    //    if(e.Action == (System.Collections.Specialized.NotifyCollectionChangedAction.Add |
    //    //        System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
    //    //        )
    //    //    {
    //    //        var propv = (TPropView)e.NewItems[0];
    //    //        Target.TargetUpdated += propv.Update;
    //    //    }
    //    //}

    //    /// <summary>
    //    /// Основной конструктор
    //    /// </summary>
    //    /// <param name="target"></param>
    //    /// <param name="propBindings"></param>
    //    public PropManagerBase(TWrapper target, List<TBind> propBindings) : this()
    //    {
    //        this.target = target;
    //        PropBindings = propBindings;
    //    }

    //    private TWrapper target;
    //    private List<TBind> propBindings;
    //    private ObservableCollection<TPropView> propViews;

    //    public abstract event EventHandler PropViewUpdated;

    //    /// <summary>
    //    /// Цель привязки
    //    /// </summary>
    //    public TWrapper Target => target; 
    //    /// <summary>
    //    /// Список привязок для представлений
    //    /// </summary>
    //    public List<TBind> PropBindings { get => propBindings; set => propBindings = value; }
    //    /// <summary>
    //    /// Список представлений
    //    /// </summary>
    //    public ObservableCollection<TPropView> PropViews => propViews;
    //    /// <summary>
    //    /// Обёртка привязки
    //    /// </summary>
    //    public TWrapper TargetWrapper => target;

    //    /// <summary>
    //    /// Очистить значения представлений
    //    /// </summary>
    //    public void ClearValues()
    //    {
    //        foreach (var val in PropViews)
    //            val.ClearValues();
    //    }
    //    /// <summary>
    //    /// Обновить значения представлений
    //    /// </summary>
    //    public void UpdateAllValues()
    //    {
    //        foreach (var val in PropViews)
    //            val.Update();
    //    }
    //    /// <summary>
    //    /// Записать значения представлений
    //    /// </summary>
    //    public void WriteAllValues()
    //    {
    //        var req = PropViews.Where(x => x.IsWritable);
    //        foreach (var val in req)
    //            if(val.IsWritable)
    //                val.WriteValue();
    //    }

    //    private void CreatePropBinding()
    //    {

    //    }
    //    /// <summary>
    //    /// Сформировать список представлений
    //    /// </summary>
    //    public abstract void CreatePropViews();


    //    /// <summary>
    //    /// Преобразование типа IEnumerator
    //    /// </summary>
    //    /// <typeparam name="T">Тип преобразования</typeparam>
    //    /// <param name="iterator">Targer IEnumerator</param>
    //    /// <returns></returns>
    //    protected static IEnumerator<T> Cast<T>(IEnumerator iterator)
    //    {
    //        while (iterator.MoveNext())
    //        {
    //            yield return (T)iterator.Current;
    //        }
    //    }
    //    public virtual IEnumerator<IPropView> GetEnumerator()
    //    {
    //        return Cast<IPropView>(PropViews.GetEnumerator());
    //    }
    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return PropViews.GetEnumerator();
    //    }

    //}

    #endregion

    //public class PropModelManager<TargetType, BindType> 
    //    : PropManagerBase<TargetType, BindType>
    //    where TargetType : IAppModel
    //    where BindType : IAppModel
    //{


    //    public override event EventHandler PropViewUpdated;

    //    public override void CreatePropViews()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void UpdateBindingTargets(TargetType newTarget)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
