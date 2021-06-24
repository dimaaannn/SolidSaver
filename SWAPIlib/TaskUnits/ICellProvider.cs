using SolidWorks.Interop.sldworks;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.TaskUnits
{
    /// <summary>
    /// Поставщик ячеек
    /// </summary>
    public interface ICellProvider 
    {
        CellGetterDelegate GetCell { get; }
    }

    /// <summary>
    /// Требуемые ключи ячеек
    /// </summary>
    public interface IRequirementKeys
    {
        HashSet<ModelEntities> Requirements { get; }
    }

    /// <summary>
    /// Шаблон для ICellFactory
    /// </summary>
    public interface ICellFactoryProvider : ICellProvider, ITableChecker, IRequirementKeys
    {
        string Name { get; set; }
        string Key { get; set; }
        bool OverrideKey { get; set; }
    }

    public class CellFactoryProvider : ICellFactoryProvider
    {
        private string name;

        public CellFactoryProvider() { }
        public CellFactoryProvider(string key, CellGetterDelegate cellGetter)
        {
            Key = key;
            GetCell = cellGetter;
        }
        public string Name { get => name ?? Key; set => name = value; }
        public string Key { get; set; } = "NoKey";
        public bool OverrideKey { get; set; } = false;
        public CellGetterDelegate GetCell { get; set; }
        public CheckTableDelegate CheckTable { get; set; } = (x, y) => true;
        public HashSet<ModelEntities> Requirements { get; set; } = new HashSet<ModelEntities>();
    }
}
