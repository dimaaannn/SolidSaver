using SWAPIlib.Table;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    /// <summary>
    /// Делегат взаимодействия с таблицей ячеек
    /// </summary>
    /// <param name="refTable">Объект взаимодействия</param>
    /// <param name="settings">Настройки (опционально)</param>
    /// <returns>Лог взаимодействия</returns>
    public delegate TableLog TableActionDelegate(ref ITable refTable, ITable settings);
    /// <summary>
    /// Делегат проверки таблицы
    /// </summary>
    /// <param name="refTable">Таблица проверки</param>
    /// <param name="settings">Опции проверки</param>
    /// <returns></returns>
    public delegate bool CheckTableDelegate(ITable refTable, ITable settings);
    /// <summary>
    /// Делегат получения (или создания) ячейки
    /// </summary>
    /// <param name="refTable">Объект извлечения (опционально)</param>
    /// <param name="settings">Свойства извлечения (опционально)</param>
    /// <returns>Ячейка</returns>
    public delegate ICell CellGetterDelegate(ITable refTable, ITable settings);

    /// <summary>
    /// Валидация таблицы
    /// </summary>
    public interface ITableChecker
    {
        CheckTableDelegate CheckTable { get; }
    }


    /// <summary>
    /// Файловый URI
    /// </summary>
    public interface IPathOption
    {
        string Path { get; set; }
    }

}
