using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Table
{
    public interface ICollectionCell<T> : 
        ICollection<T>, 
        ICell<IEnumerable<T>> 
    {

    }

    public class CollectionCell<T> : BaseCell, ICollectionCell<T>
    {
        private HashSet<T> dataStorage = new HashSet<T>();

        public virtual bool IsReadOnly { get; protected set; }

        public int Count => dataStorage.Count;
        public IEnumerable<T> Value => dataStorage.AsEnumerable();

        /// <summary>
        /// Не реализовано в данном классе
        /// </summary>
        /// <returns></returns>
        public override bool Update() => true;

        #region CollectionInterfaceProxy
        public void Add(T item) => dataStorage.Add(item);
        public void Clear() => dataStorage.Clear();
        public bool Contains(T item) => dataStorage.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) =>
            dataStorage.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => dataStorage.GetEnumerator();
        public bool Remove(T item) => dataStorage.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion
    }
}
