using SWAPIlib.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    public class ActionList : IEnumerable<ICellFactory>
    {
        private List<ICellFactory> CellFactories { get; } = new List<ICellFactory>();


        public IExtendedTable Build(ITable table)
        {

        }

        public void Add(ICellFactory factory) => CellFactories.Add(factory);
        public void Remove(ICellFactory factory) => CellFactories.Remove(factory);
        public void Clear() => CellFactories.Clear();

        public IEnumerator<ICellFactory> GetEnumerator() => CellFactories.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => CellFactories.GetEnumerator();

    }


    public static class TableBuilderExtension
    {
        public static void AddTo(this ICellFactory factory, ActionList tableBuilder)
        {
            tableBuilder.Add(factory);
        }
    }
}
