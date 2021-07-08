using Ninject;
using SWAPIlib.BaseTypes;
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
            Bind<IExtendedTableFactory>().To<ExtendedTableFactory>().InSingletonScope();
            Bind<ITableCollection>().To<TableCollection>().InTransientScope();
            Bind<SelectedModelProvider>().To<SelectedModelProvider>().InSingletonScope();
            Bind<IPartProvider<IComponentWrapper>>().To<SelectedComponentProvider>().InSingletonScope();

        }
    }


    public interface IActionUnit
    {
        IActionUnitResult Proceed(IExtendedTable table);
    }

    public interface IActionUnitResult
    {
    }

    public interface ITableCollection : ICollection<IExtendedTable>
    {
        bool GetFromProvider(IPartProvider partProvider);
    }

    public interface IPartProvider
    {
        IEnumerable<IPartWrapper> GetTargets();
    }

    public interface IPartProvider<T> : IPartProvider
    {
        new IEnumerable<T> GetTargets();
    }

    public class SelectedComponentProvider : IPartProvider<IComponentWrapper>
    {
        private readonly IPartWrapperFactory partWrapperFactory;
        public SelectedComponentProvider(IPartWrapperFactory partWrapperFactory)
        {
            this.partWrapperFactory = partWrapperFactory;
        }
        public IEnumerable<IComponentWrapper> GetTargets()
        {
            var ret = new List<IComponentWrapper>();

            foreach (var component in SWAPIlib.Global.MainModel.SelectionList)
            {
                var modelWrapper = partWrapperFactory.GetComponentWrapper(component.Appmodel.SwCompModel);
                ret.Add(modelWrapper);
            }
            return ret;
        }
        IEnumerable<IPartWrapper> IPartProvider.GetTargets() => GetTargets();
    }

    public class SelectedModelProvider : IPartProvider<IModelWrapper>
    {
        private readonly IPartProvider<IComponentWrapper> selectedCompProvider;

        public SelectedModelProvider(IPartProvider<IComponentWrapper> selectedCompProvider)
        {
            this.selectedCompProvider = selectedCompProvider ?? throw new ArgumentNullException(nameof(selectedCompProvider));
        }
        public IEnumerable<IModelWrapper> GetTargets()
        {
            return selectedCompProvider.GetTargets().Select(comp => comp.GetModel());
        }
        IEnumerable<IPartWrapper> IPartProvider.GetTargets() => GetTargets();
    }

    /// <summary>
    /// Temp class for run test
    /// </summary>
    public class TableProvider //TODO refactor provider
    {
        public ITableCollection UserSelectedModels()
        {
            var tableCollection = Initialiser.kernel.Get<ITableCollection>();
            var partProvider = Initialiser.kernel.Get<SelectedModelProvider>();
            tableCollection.GetFromProvider(partProvider);
            return tableCollection;
        }
    }
    public class TableCollection : ITableCollection
    {
        private readonly List<IExtendedTable> mainTableList = new List<IExtendedTable>();
        private readonly IExtendedTableFactory extendedTableFactory;

        private List<IExtendedTable> MainTableList => mainTableList;

        public TableCollection(IExtendedTableFactory extendedTableFactory)
        {
            this.extendedTableFactory = extendedTableFactory;
        }
        public bool GetFromProvider(IPartProvider partProvider)
        {
            IEnumerable<IPartWrapper> parts = partProvider.GetTargets();
            if(parts.Count() > 0)
            {
                MainTableList.AddRange(parts.Select(part => extendedTableFactory.Get(part)));
                return true;
            }
            return false;
        }

        #region CollectionInterface
        public bool IsReadOnly => false;
        public void Add(IExtendedTable item) => MainTableList.Add(item);
        public bool Remove(IExtendedTable item) => MainTableList.Remove(item);
        public int Count => MainTableList.Count();


        public void Clear() => MainTableList.Clear();
        public bool Contains(IExtendedTable item) => MainTableList.Contains(item);
        public void CopyTo(IExtendedTable[] array, int arrayIndex) => MainTableList.CopyTo(array, arrayIndex);
        public IEnumerator<IExtendedTable> GetEnumerator() => MainTableList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => MainTableList.GetEnumerator(); 
        #endregion


    }

    public interface IExtendedTableFactory
    {
        IExtendedTable Get(ITarget2 target = null);
    }

    public class ExtendedTableFactory : IExtendedTableFactory
    {
        public IExtendedTable Get(ITarget2 target = null)
        {
            return new ExtendedTable { Target = target };
        }
    }
}
