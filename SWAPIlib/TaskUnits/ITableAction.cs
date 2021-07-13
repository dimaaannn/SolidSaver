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
        ITable OptionalSettings { get; set; }
    }


    public abstract class TableActionBase : ITableAction
    {
        private Func<ITable, ITable> getSettingsDelegate;

        public Func<ITable, ITable> GetSettingsDelegate 
        {
            get => getSettingsDelegate ?? (getSettingsDelegate = _ => OptionalSettings); 
            set => getSettingsDelegate = value; 
        }


        public ITable OptionalSettings { get; set; }
        public TableLog Proceed(ref ITable refTable)
        {
            var settings = GetSettingsDelegate?.Invoke(refTable);
            return Proceed(ref refTable, settings);
        }

        public abstract string Name { get; }
        public abstract TableLog Proceed(ref ITable refTable, ITable settings);
    }

}
