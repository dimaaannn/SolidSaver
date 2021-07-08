using SWAPIlib.BaseTypes;
using SWAPIlib.Table;

namespace SWAPIlib.TaskCollection
{
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
