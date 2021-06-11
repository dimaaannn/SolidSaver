using SWAPIlib.Property;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Table
{
    public interface ITable : IEnumerable<KeyValuePair<string, ICell>>
    {
        ICell GetCell(string cellKey);
        void Add(string cellKey, ICell cell);
    }

    public interface ITargetTable : ITable
    {
        object GetTarget();
    }


    public abstract class BaseTable : ITable
    {
        protected Dictionary<string, ICell> Cells = new Dictionary<string, ICell>();

        public virtual void Add(string cellKey, ICell cell) => Cells.Add(cellKey, cell);

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

    public class Table : BaseTable, ITargetTable
    {
        private readonly object targetObj;

        public Table(object targetObj)
        {
            this.targetObj = targetObj;
        }

        public object GetTarget()
        {
            return targetObj;
        }
    }
}
