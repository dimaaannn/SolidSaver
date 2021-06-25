using SWAPIlib.Table;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    public interface ITaskUnit
    {
        ITable GetSettings(ITable refTable);
        bool Proceed(ref ITable refTable);

        IFactoryProvider FactoryProvider { get; }
    }

}
