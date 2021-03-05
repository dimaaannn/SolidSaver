using SWAPIlib.Controller;
using SWAPIlib.MProperty;
using SWAPIlib.PropertyObj;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Global
{
    public interface IMainPartControl 
    {
        /// <summary>
        /// Загруженная модель
        /// </summary>
        ILinkedModel LinkedRootModel { get; set; }
        /// <summary>
        /// Список компонентов корневой сборки (тестовый)
        /// </summary>
        ObservableCollection<SWAPIlib.Controller.IComponentControl> RootComponents { get; }
        int ActiveSelectionGroup { get; set; }

        /// <summary>
        /// Список отмеченных компонентов
        /// </summary>
        /// <returns></returns>
        IEnumerable<IComponentControl> GetCheckedComponents();
        void CheckAllComponents();
        void ClearSelection();

    }

    public class MainPartControl : IMainPartControl
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="linkedModel"></param>
        public MainPartControl(ILinkedModel linkedModel)
        {
            LinkedRootModel = linkedModel;

        }
        /// <summary>
        /// Ссылка на коренную модель
        /// </summary>
        public ILinkedModel LinkedRootModel { get; set; }

        /// <summary>
        /// Список дочерних компонентов
        /// </summary>
        public ObservableCollection<IComponentControl> RootComponents
        {
            get
            {
                if (LinkedRootModel != null && LinkedRootModel.IsHaveSubComponents == null)
                    LinkedRootModel.GetSubComponents();
                return LinkedRootModel.SubComponents.SubComponents; // { get; private set; }
            }
        }

        public int ActiveSelectionGroup { get; set; }

        public void CheckAllComponents()
        {
            if (LinkedRootModel?.IsHaveSubComponents == true)
                LinkedRootModel.SubComponents.Select(x => x.IsSelected = true);
        }

        public void ClearSelection()
        {
            if (LinkedRootModel?.IsHaveSubComponents == true)
                LinkedRootModel.SubComponents.Select(x => x.IsSelected = false);
        }

        public IEnumerable<IComponentControl> GetCheckedComponents() => LinkedRootModel?.SubComponents.Where(x => x.IsSelected).AsEnumerable();
        
    }

    /// <summary>
    /// Ienumerator для компонентов
    /// </summary>
    public class AssemblyTree : IEnumerator<IComponentControl>, IEnumerable<IComponentControl>
    {

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="appAssembly"></param>
        public AssemblyTree(IAppAssembly appAssembly, Func<IComponentControl, bool> compFilter = null)
        {
            SubComponents = new ObservableCollection<IComponentControl>();
            SubComponentFilter = compFilter;

            foreach (var comp in appAssembly.GetComponents(true))
            {
                var compControl = new SWAPIlib.Controller.ComponentControl(comp);
                SubComponents.Add(compControl);
            }

        }

        private IAppModel NodeModel;
        public ObservableCollection<IComponentControl> SubComponents { get; set; }

        /// <summary>
        /// Фильтр загрузки компонентов
        /// </summary>
        public Func<IComponentControl, bool> SubComponentFilter;

        #region IEnumerator
        private int _CurrentNum = 0;
        private bool _IsTopLevelObjReturned = false;

        public IComponentControl Current { get; private set; }
        object IEnumerator.Current => Current;

        public virtual bool MoveNext()
        {
            //if (_CurrentNum < 0)
            //{
            //    _CurrentNum++;
            //    if (NodeModel is IComponentControl nodeComp
            //        && (SubComponentFilter?.Invoke(nodeComp) ?? true))
            //    {

            //        Current = nodeComp;
            //        return true;
            //    }
            //}
            while (_CurrentNum < SubComponents.Count)
            {
                if(!_IsTopLevelObjReturned)
                {
                    _IsTopLevelObjReturned = true;
                    Current = SubComponents[_CurrentNum];
                    return true;
                }
                if (SubComponents[_CurrentNum].MoveNext())
                {
                    Current = SubComponents[_CurrentNum].Current;

                    if (SubComponentFilter?.Invoke(Current) ?? true)
                        return true;
                    else
                        continue;
                }
                else
                {
                    _IsTopLevelObjReturned = false;
                    _CurrentNum++;
                }
            }
            return false;
        }

        public void Reset() { _CurrentNum = 0; Current = null; }
        public void Dispose() => Reset();

        public IEnumerator<IComponentControl> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
        #endregion

        /// <summary>
        /// Компонент под номером
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IComponentControl this[int index] => SubComponents[index];

    }

}
