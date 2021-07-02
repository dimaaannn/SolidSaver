using NLog;
using SWAPIlib.Table;
using System;

namespace SWAPIlib.TaskUnits
{
    /// <summary>
    /// Взаимодействие с таблицей
    /// </summary>
    public interface ITableAction
    {
        string Name { get; }
        TableLog Proceed(ref ITable refTable, ITable settings);
        TableLog Proceed(ref ITable refTable);

        Func<ITable, ITable> Settings { get; set; }
    }


    public abstract class TableActionBase : ITableAction
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Func<ITable, ITable> Settings { get; set; }
        public TableLog Proceed(ref ITable refTable) => Proceed(ref refTable, Settings?.Invoke(refTable));

        public abstract string Name { get; }
        public abstract TableLog Proceed(ref ITable refTable, ITable settings);
    }

}
