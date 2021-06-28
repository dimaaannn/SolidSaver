using SWAPIlib.Table;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    public interface ITaskUnit
    {
        bool Proceed(ref ITable refTable);

        IActionProvider ActionProvider { get; set; }
    }

    public abstract class TaskUnitBase 
    {
        private IFactoryProvider factoryProvider = new FactoryProvider();
        public CellFactoryTemplate CellFactoryTemplate { get; protected set; }

        protected IFactoryProvider FactoryProvider { get => factoryProvider; set => factoryProvider = value; }

        public abstract bool Proceed(ref ITable refTable);
        
    }

}
