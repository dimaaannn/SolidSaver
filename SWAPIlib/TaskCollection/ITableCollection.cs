using SWAPIlib.BaseTypes;
using SWAPIlib.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.TaskCollection
{
    public interface ITableCollection : ICollection<IExtendedTable>
    {
        bool GetFromProvider(ITargetProvider partProvider);
        Func<ITarget2, bool> TargetFilter { get; set; }
    }

    public class TableCollection : ITableCollection
    {
        private readonly List<IExtendedTable> mainTableList = new List<IExtendedTable>();
        private readonly ITaskServices taskServices;

        private List<IExtendedTable> MainTableList => mainTableList;
        public Func<ITarget2, bool> TargetFilter { get; set; }

        public TableCollection(ITaskServices taskServices)
        {
            this.taskServices = taskServices ?? throw new ArgumentNullException(nameof(taskServices));
        }
        public bool GetFromProvider(ITargetProvider partProvider)
        {
            IEnumerable<ITarget2> parts;
            if (TargetFilter != null)
                parts = partProvider.GetTargets().Where(TargetFilter);
            else
                parts = partProvider.GetTargets();

            if(parts.Count() > 0)
            {
                foreach (ITarget2 target in parts)
                {
                    if(target != null)
                    {
                        var extendedTable = taskServices.CreateExtendedTable(target);
                        MainTableList.Add(extendedTable);
                    }
                }
                return true;
            }
            return false;
        }

        #region CollectionInterface
        public bool IsReadOnly => false;
        public void Add(IExtendedTable item) => MainTableList.Add(item);
        public bool Remove(IExtendedTable item) => MainTableList.Remove(item);
        public int Count => MainTableList.Count();


        public void Clear() => MainTableList.Clear();
        public bool Contains(IExtendedTable item) => MainTableList.Contains(item);
        public void CopyTo(IExtendedTable[] array, int arrayIndex) => MainTableList.CopyTo(array, arrayIndex);
        public IEnumerator<IExtendedTable> GetEnumerator() => MainTableList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => MainTableList.GetEnumerator(); 
        #endregion


    }
}
