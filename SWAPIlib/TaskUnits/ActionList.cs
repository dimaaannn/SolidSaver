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
    public class ActionList : IEnumerable<ITableAction>
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
        public void Remove(ITableAction factory) => tableActions.Remove(factory);
        public void Clear() => tableActions.Clear();

        public IEnumerator<ITableAction> GetEnumerator() => tableActions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => tableActions.GetEnumerator();

    }


    public static class TableBuilderExtension
    {
        public static void AddTo(this ITableAction factory, ActionList tableBuilder)
        {
            tableBuilder.Add(factory);
        }
    }
}
