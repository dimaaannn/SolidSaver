﻿using SWAPIlib.Table;
using System.Collections.Generic;

namespace SWAPIlib.TaskUnits
{

    public enum ITableType
    {
        Default,
        Settings
    }


    public interface IActionTarget
    {
        ITable GetTable(ITableType tableType = ITableType.Default);
        void Add(ITable newTable, ITableType tableType = ITableType.Default);

        T GetTarget<T>();
    }


    public class ActionTarget : IActionTarget
    {
        private readonly Dictionary<ITableType, ITable> tableDictionary =
            new Dictionary<ITableType, ITable>();

        public ActionTarget() { }

        public ActionTarget(ITable defaultTable)
        {
            tableDictionary.Add(ITableType.Default, defaultTable);
        }


        public ITargetWrapper TargetWrapper { get; set; }

        public void Add(ITable newTable, ITableType tableType = ITableType.Default)
        {
            tableDictionary[tableType] = newTable;
        }

        public ITable GetTable(ITableType tableType = ITableType.Default)
        {
            ITable ret;
            bool isSuccess = tableDictionary.TryGetValue(tableType, out ret);
            return ret;
        }

        public T GetTarget<T>()
        {
            return (T)TargetWrapper.GetTarget();
        }

    }

}
