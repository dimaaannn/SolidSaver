using NLog;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWAPIlib.TaskUnits
{
    public class AddCellAction : TableActionBase, ITableChecker
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ICellFactoryProvider cellProvider;

        public AddCellAction(ICellFactoryProvider cellProvider)
        {
            this.cellProvider = cellProvider ?? throw new ArgumentNullException(nameof(cellProvider));
        }

        public override string Name => cellProvider.Name;
        public CheckTableDelegate CheckTable => cellProvider.CheckTable;

        

        public override TableLog Proceed(ref ITable refTable, ITable settings)
        {
            logger.Debug("Proceed table {tableName}", refTable.Name);
            TableLog tlog = new TableLog
            {
                Info = "AddCellAction",
                Log = new List<CellLog>(),
                Status = LogStatus.None
            };

            CellLog log = GetCell(cellProvider, refTable, settings);
            tlog.Log.Add(log);

            if (log)
            {
                tlog.Log.Add(
                    AddToTable(log.Cell, cellProvider.Key, refTable));
            }

            return tlog;
        }

        private CellLog GetCell(
            ICellFactoryProvider cellFactoryProvider, 
            ITable refTable, 
            ITable settings)
        {
            if (cellFactoryProvider is null)
            {
                logger.Error("CellFactoryProvider cannot be null");
                throw new ArgumentNullException(nameof(cellFactoryProvider));
            }

            CellLog log = new CellLog
            {
                Time = DateTime.Now,
                Action = CellAction.Create,
                Info = "Получение ячейки из конструктора",
            };

            if(cellProvider.CheckTable(refTable: refTable, settings: settings))
            {
                log.Cell = cellProvider.GetCell(refTable, settings);
                if(log.Cell != null)
                {
                    logger.Trace("Cell {name} created from {tableName}", cellProvider.Name, refTable.Name);
                    log.Status = LogStatus.Processed;
                }
                else
                {
                    logger.Warn("Cell {name} return null from {tableName}", cellProvider.Name, refTable.Name);
                    log.Status = LogStatus.Failed;
                }
            }
            else
            {
                log.Status = LogStatus.Passed;
                logger.Debug("Cell {name} is not valid for {tableName}", cellProvider.Name, refTable.Name);
            }
            return log;
        }

        private CellLog AddToTable(ICell cell, string key, ITable refTable)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty", nameof(key));
            }

            if (refTable is null)
            {
                throw new ArgumentNullException(nameof(refTable));
            }

            logger.Trace("Add cell {cellName} to table {table}", cellProvider.Name, refTable.Name);
            CellLog cellLog = new CellLog
            {
                Time = DateTime.Now,
                Action = CellAction.Add,
                Info = "Добавление новой ячейки в таблицу"
            };

            if (cell != null)
            {
                refTable.Add(cellKey: key, cell);
                cellLog.Status = LogStatus.Processed;
                cellLog.Cell = cell;
                logger.Trace("Cell is added to {table}  with key {key}",  refTable.Name, key);
            }
            else
            {
                logger.Debug("Cell with key {key} is null, passed", key);
                cellLog.Status = LogStatus.Passed;
            }

            return cellLog;
        }

    }
}
