using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{

    public class TaskCollectionNinjectBinding : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<ITableProvider>().To<TableProvider>();

        }
    }


    public interface IActionUnit
    {
        IActionUnitResult Proceed(IExtendedTable table);
    }

    public interface IActionUnitResult
    {
    }

    public interface ITableProvider : ICollection<IExtendedTable>
    {
        void AddTargets(IEnumerable<ITargetWrapper> targets);
    }

    public interface ITargetProvider
    {
        IEnumerable<ITargetWrapper> GetTargets();
    }

    public class SelectedComponentProvider : ITargetProvider
    {
        public IEnumerable<ITargetWrapper> GetTargets()
        {
            var ret = new List<ITargetWrapper>();

            foreach (var component in SWAPIlib.Global.MainModel.SelectionList)
            {
                var target = new TargetWrapper(component.Appmodel);
            }
            return ret;
        }
    }

    public class TableProvider : ITableProvider
    {
        public TableProvider(ITargetProvider targetProvider)
        {
            this.targetProvider = targetProvider;
        }
        private readonly ITargetProvider targetProvider;
        private readonly List<IExtendedTable> mainTableList = new List<IExtendedTable>();

        public void AddTargets(IEnumerable<ITargetWrapper> targets)
        {

        }

        #region CollectionInterface
        public bool IsReadOnly => false;
        public void Add(IExtendedTable item) => mainTableList.Add(item);
        public bool Remove(IExtendedTable item) => mainTableList.Remove(item);
        public int Count => mainTableList.Count();
        public void Clear() => mainTableList.Clear();
        public bool Contains(IExtendedTable item) => mainTableList.Contains(item);
        public void CopyTo(IExtendedTable[] array, int arrayIndex) => mainTableList.CopyTo(array, arrayIndex);
        public IEnumerator<IExtendedTable> GetEnumerator() => mainTableList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => mainTableList.GetEnumerator(); 
        #endregion


    }
}
