using Ninject;
using SWAPIlib.BaseTypes;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{
    public interface ITaskServices
    {
        IExtendedTable NewExtendedTable { get; }
        ITableCollection NewTableCollection { get;  }
        ISelectedComponentProvider UserSelectedComponentsProvider { get;  }
        ISelectedModelProvider UserSelectedModelsProvider { get; }
    }

    public class TaskServices : ITaskServices
    {
        [Inject]
        public ISelectedModelProvider UserSelectedModelsProvider { get; set; }
        [Inject]
        public ISelectedComponentProvider UserSelectedComponentsProvider { get; set; }
        [Inject]
        public ITableCollection NewTableCollection { get; set; }
        [Inject]
        public IExtendedTable NewExtendedTable { get; set; }

    }

}
