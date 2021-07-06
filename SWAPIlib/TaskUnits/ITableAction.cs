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

        Func<ITable, ITable> GetSettingsDelegate { get; set; }
    }


    public abstract class TableActionBase : ITableAction
    {
        public Func<ITable, ITable> GetSettingsDelegate { get; set; }
        public TableLog Proceed(ref ITable refTable) => Proceed(ref refTable, GetSettingsDelegate?.Invoke(refTable));

        public abstract string Name { get; }
        public abstract TableLog Proceed(ref ITable refTable, ITable settings);
    }

}
