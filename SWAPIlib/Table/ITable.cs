using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Table
{
    public interface ITable : IEnumerable<KeyValuePair<string, ICell>>
    {
        ICell GetCell(string cellKey);
    }

    public interface ITargetTable : ITable
    {
        SWAPIlib.Property.ITarget GetTarget();
    }


    public abstract class BaseTable : ITable
    {
        protected Dictionary<string, ICell> Cells = new Dictionary<string, ICell>();

        public virtual ICell GetCell(string cellKey)
        {
            ICell ret;
            Cells.TryGetValue(cellKey, out ret);
            return ret;
        }

        public IEnumerator<KeyValuePair<string, ICell>> GetEnumerator()
        {
            return Cells.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
