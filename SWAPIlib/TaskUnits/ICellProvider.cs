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
        /// <summary>
        /// Имя фабрики
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Ключ добавления в таблицу
        /// </summary>
        string Key { get; set; }
        /// <summary>
        /// Перезаписать при совпадении ключей
        /// </summary>
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
        /// <summary>
        /// Ключ добавления в таблицу
        /// </summary>
        public string Key { get; set; } = "NoKey";
        /// <summary>
        /// Перезаписать ключ в случае совпадения
        /// </summary>
        public bool OverrideKey { get; set; } = false;
        /// <summary>
        /// Делегат создания ячейки
        /// </summary>
        public CellGetterDelegate GetCell { get; set; }
        /// <summary>
        /// Проверка таблицы перед добавлением
        /// </summary>
        public CheckTableDelegate CheckTable { get; set; } = (x, y) => true;
        /// <summary>
        /// Список требуемых ключей для ссылок
        /// </summary>
        public HashSet<ModelEntities> Requirements { get; set; } = new HashSet<ModelEntities>();
    }
}
