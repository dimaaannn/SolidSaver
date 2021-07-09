using SWAPIlib.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.TaskCollection
{
    public interface ITargetProvider
    {
        IEnumerable<ITarget2> GetTargets();
    }

    public interface ITargetProvider<T> : ITargetProvider where T : ITarget2
    {
        new IEnumerable<T> GetTargets();
    }

    public class SelectedComponentProvider : ITargetProvider<IComponentWrapper>
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

        IEnumerable<ITarget2> ITargetProvider.GetTargets() => GetTargets();
    }

    public class SelectedModelProvider : ITargetProvider<IModelWrapper>
    {
        private readonly ITargetProvider<IComponentWrapper> selectedCompProvider;

        public SelectedModelProvider(ITargetProvider<IComponentWrapper> selectedCompProvider)
        {
            this.selectedCompProvider = selectedCompProvider ?? throw new ArgumentNullException(nameof(selectedCompProvider));
        }
        public IEnumerable<IModelWrapper> GetTargets()
        {
            return selectedCompProvider.GetTargets().Select(comp => comp.GetModel());
        }
        IEnumerable<ITarget2> ITargetProvider.GetTargets() => GetTargets();
    }
}
