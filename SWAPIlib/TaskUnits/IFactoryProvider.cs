using SWAPIlib.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.TaskUnits
{


    public interface IActionProvider : IEnumerable<ITableAction>
    {

    }


    public interface IFactoryProvider : IActionProvider
    {
        ICellFactory GetFactory(string key);
        IEnumerable<ICellFactory> GetFactories();
        void Add(ICellFactory factory);
    }


    /// <summary>
    /// Класс для хранения списка обработчиков
    /// </summary>
    public class FactoryProvider : IFactoryProvider
    {

        private readonly List<ICellFactory> factories;
        public FactoryProvider()
        {
            factories = new List<ICellFactory>();
        }

        public void Add(ICellFactory factory)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

        }
        public IEnumerable<ICellFactory> GetFactories() => factories;

        /// <summary>
        /// Фабрика из внутреннего списка
        /// </summary>
        /// <param name="key">ключ из шаблона</param>
        /// <returns>Null if not found</returns>
        /// ArgumentNullException if key null or empty
        public ICellFactory GetFactory(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("IFactoryProvider: key cannot be null or empty");
            return GetFactoryFromList(factories, key);
        }

        private static bool CheckFactoryKeyExist(IEnumerable<ICellFactory> factoryList, ICellFactory factory)
        {
            string key = factory.CellProvider.Key;
            return factoryList.Any(f => f.CellProvider.Key == key);
        }

        private static ICellFactory GetFactoryFromList(IList<ICellFactory> cellFactories, string key)
        {
            return cellFactories.FirstOrDefault(factory => factory.CellProvider.Key == key);
        }

        public IEnumerator<ITableAction> GetEnumerator() => factories.Cast<ITableAction>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => factories.GetEnumerator();
    }

    //public static class FactoryProviderExtension
    //{
    //    /// <summary>
    //    /// Добавить текстовую ячейку
    //    /// </summary>
    //    /// <param name="fprovider"></param>
    //    /// <param name="key">Ключ</param>
    //    /// <param name="textValue">Шаблон текста</param>
    //    /// <param name="overrideKey">Перезаписать значение</param>
    //    /// <returns></returns>
    //    public static IFactoryProvider Add(
    //       this IFactoryProvider fprovider,
    //       string key,
    //       string textValue,
    //       bool overrideKey)
    //    {
    //        if (fprovider is null)
    //            throw new ArgumentNullException(nameof(fprovider));
    //        if (key is null)
    //            throw new ArgumentNullException(nameof(key));

    //        ICellFactoryProvider provider = BuildNewTextProvider(key, textValue, overrideKey);
    //        ICellFactory factory = CreateNewFactory(provider);
    //        fprovider.Add(factory, overrideKey);
    //        return fprovider;
    //    }

    //    /// <summary>
    //    /// Добавить ссылку на ячейку
    //    /// </summary>
    //    /// <param name="key">Ключ</param>
    //    /// <param name="referencedCell">Ссылка на ячейку</param>
    //    /// <param name="overrideKey">Перезаписать значение в случае конфликта</param>
    //    /// <param name="checkTableDelegate">Проверка таблицы</param>
    //    public static IFactoryProvider Add(
    //        this IFactoryProvider fprovider,
    //        string key,
    //        ICell referencedCell,
    //        bool overrideKey = true,
    //        CheckTableDelegate checkTableDelegate = null)
    //    {
    //        if (fprovider is null)
    //            throw new ArgumentNullException(nameof(fprovider));
    //        if (key is null)
    //            throw new ArgumentNullException(nameof(key));
    //        if (referencedCell is null)
    //            throw new ArgumentNullException(nameof(referencedCell));

    //        ICellFactoryProvider provider = ReferenceToCellProvider(
    //            key: key,
    //            cell: referencedCell,
    //            overrideKey: overrideKey,
    //            checkTable: checkTableDelegate);

    //        ICellFactory factory = CreateNewFactory(provider);

    //        fprovider.Add(factory, overrideKey);
    //        return fprovider;
    //    }


    //    public static IFactoryProvider AddFromTemplate(
    //        this IFactoryProvider fprovider,
    //        ICellFactoryTemplate cellProviderTemplate,
    //        ModelPropertyNames propertyName,
    //        string Key = null,
    //        bool overridekey = false)
    //    {
    //        if (fprovider is null)
    //            throw new ArgumentNullException(nameof(fprovider));

    //        var factory = GetFactoryFromTemplate(cellProviderTemplate, propertyName, Key);
    //        if(factory != null)
    //        {
    //            fprovider.Add(factory, overridekey);
    //        }
    //        return fprovider;
    //    }

    //    public static IFactoryProvider


    //    private static ICellFactory CreateNewFactory(ICellFactoryProvider cellFactoryProvider)
    //    {
    //        if (cellFactoryProvider?.Key is null)
    //            throw new ArgumentNullException(nameof(cellFactoryProvider));
    //        return new CellFactory(cellFactoryProvider);
    //    }

    //    private static ICellFactoryProvider ReferenceToCellProvider(string key, ICell cell, bool overrideKey = true, CheckTableDelegate checkTable = null)
    //    {
    //        var ret = new CellFactoryProvider
    //        {
    //            Name = "ReferenceToCell",
    //            Key = key,
    //            OverrideKey = overrideKey,
    //            GetCell = (table, settings) => cell
    //        };

    //        if (checkTable != null)
    //            ret.CheckTable = checkTable;

    //        return ret;
    //    }

    //    private static ICellFactoryProvider BuildNewTextProvider(string key, string textValue, bool overrideKey)
    //    {
    //        return new CellFactoryProvider
    //        {
    //            Name = "Текст",
    //            Key = key,
    //            OverrideKey = overrideKey,
    //            GetCell = (table, settings) => new TextCell(textValue)
    //        };
    //    }

    //    private static ICellFactory GetFactoryFromTemplate(ICellFactoryTemplate factoryTemplate, ModelPropertyNames propertyName, string changedKey)
    //    {
    //        if (factoryTemplate is null)
    //            throw new ArgumentNullException(nameof(factoryTemplate));

    //        var cellProviderTemplate = factoryTemplate.GetCellProvider(propertyName);

    //        if (cellProviderTemplate != null)
    //        {
    //            if (string.IsNullOrEmpty(changedKey) == false)
    //            {
    //                cellProviderTemplate.Key = changedKey;
    //            }
    //            return new CellFactory(cellProviderTemplate);
    //        }
    //        else
    //            return null;
    //    }

    //}


}
