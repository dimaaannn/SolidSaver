using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table
{
    public interface ITargetWrapper
    {
        object GetTarget();
    }

    public class TargetWrapper : ITargetWrapper
    {
        public TargetWrapper(object target)
        {
            Target = target;
        }

        public TargetWrapper(IAppComponent component)
        {
            Target = component.SwModel;
        }
        public TargetWrapper(IAppModel appModel)
        {
            Target = appModel.SwModel;
        }

        protected object Target { get; set; }
        public object GetTarget()
        {
            return Target;
        }
    }
}
