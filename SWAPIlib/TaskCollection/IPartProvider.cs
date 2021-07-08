using SWAPIlib.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.TaskCollection
{
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
}
