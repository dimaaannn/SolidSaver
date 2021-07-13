using NLog;
using SWAPIlib.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    public interface IActionList : IEnumerable<ITableAction>
    {
        TableLog Proceed(ITable table);

        void Add(ITableAction factory);
        void AddRange(IEnumerable<ITableAction> factories);
        void Clear();
        void Remove(ITableAction factory);
    }

    public class ActionList : IEnumerable<ITableAction>, IActionList
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly List<ITableAction> tableActions = new List<ITableAction>();


        public TableLog Proceed(ITable table)
        {
            logger.Debug("Proceed table {tableName}", table.Name);
            TableLog ret = TableLog.New;
            foreach (var action in tableActions)
            {
                ret.Add(action.Proceed(ref table));
            }

            return ret;
        }

        public void Add(ITableAction factory) => tableActions.Add(factory);
        public static ActionList DefaultBuilder(Action<ActionList> builderDelegate)
        {
            ActionList ret = new ActionList();
            builderDelegate(ret);
            return ret;
        }

        public void AddRange(IEnumerable<ITableAction> factories) => tableActions.AddRange(factories);
        public void Remove(ITableAction factory) => tableActions.Remove(factory);
        public void Clear() => tableActions.Clear();

        public IEnumerator<ITableAction> GetEnumerator() => tableActions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => tableActions.GetEnumerator();

    }

}
