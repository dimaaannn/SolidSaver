using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Table
{
    public interface ITable : IEnumerable<KeyValuePair<string, ICell>>
    {
        string Name { get; }
        ICell GetCell(string cellKey);
        void Add(string cellKey, ICell cell, bool replaceVal = true);
        void CopyTo(ITable other, bool overrideKey);
    }

    public interface ITargetTable : ITable
    {
        object GetTarget();
    }


    public abstract class BaseTable : ITable
    {
        protected Dictionary<string, ICell> Cells = new Dictionary<string, ICell>();

        public virtual string Name { get; set; }

        public virtual void Add(string cellKey, ICell cell, bool replaceVal)
        {
            if (replaceVal || !Cells.ContainsKey(cellKey))
            {
                Cells[cellKey] = cell;
            }
        }

        public void CopyTo(ITable other, bool overrideKey)
        {
            foreach (var keyval in Cells)
            {
                other.Add(keyval.Key, keyval.Value, overrideKey);
            }
        }

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

    public class TableList : BaseTable
    {

    }

    public class TargetTable : BaseTable, ITargetTable
    {
        private readonly object targetObj;

        public TargetTable(object targetObj)
        {
            this.targetObj = targetObj;
        }

        public object GetTarget()
        {
            return targetObj;
        }
    }
}
