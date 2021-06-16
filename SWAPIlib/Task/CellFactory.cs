using SWAPIlib.Table;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.Task
{
    public class CellFactory
    {
        public CellFactory()
        {
            Proceed = BuildCells;
        }
        protected static ICellLogger Logger = new SimpleCellLogger<CellFactory>();
        public Dictionary<string, ICellFactory> CellFactories { get; set; }
        public TableActionDelegate Proceed { get; }
        public bool OverrideKeys { get; set; } = false;

        public bool CheckValidation(ICellFactory factory, ITable target, ITable settings = null)
        {
            bool ret = factory.CheckTarget(target, settings);
            return ret;
        }

        public CellLog CreateCell(ICellFactory factory, ITable refTable, ITable settings = null)
        {
            ICell cell = null;
            bool isValid = CheckValidation(factory: factory, target: refTable, settings: settings);

            if (isValid)
            {
                cell = factory.Build(refTable: refTable, settings: settings);
            }

            CellLog ret = Logger.Log(cell, settings);
            ret.Info = factory.Name;
            ret.Status = isValid ? LogStatus.Processed : LogStatus.Failed;

            return ret;
        }

        /// <summary>
        /// Добавить ячейку в таблицу
        /// </summary>
        /// <param name="creationLog">Лог добавляемой ячейки</param>
        /// <param name="key">Ключ ячейки</param>
        /// <param name="refTable">Таблица добавления</param>
        /// <param name="overrideKey">Переписать значение ключа если существует </param>
        /// <returns>Лог операции</returns>
        public CellLog AddCellToTable(CellLog creationLog, string key, ref ITable refTable, bool overrideKey = true)
        {
            ICell cellInTable;
            var ret = Logger.Log(creationLog.Cell, CellAction.Add);
            ret.Info = $"{key}:{creationLog.Info}";

            if (creationLog.Status == LogStatus.Processed)
            {
                refTable.Add(key, creationLog.Cell, overrideKey);
                cellInTable = refTable.GetCell(key);

                if (cellInTable != creationLog.Cell)
                {
                    if (overrideKey)
                        ret.Status = LogStatus.Passed;
                    else
                        ret.Status = LogStatus.Failed;
                }
                else
                    ret.Status = LogStatus.Processed;

            }

            return ret;
        }

        public TableLog AddAllCells(Dictionary<string, ICellFactory> factories, ITable refTable, ITable settings, bool overrideKey)
        {
            TableLog log = Logger.Log(table: refTable, settings: settings);

            bool validation;
            CellLog createLog;
            CellLog addLog;
            ICellFactory factory;
            string key;
            foreach (var keyVal in factories)
            {
                key = keyVal.Key;
                factory = keyVal.Value;

                validation = CheckValidation(factory, refTable, settings);
                if (validation)
                {
                    createLog = CreateCell(factory: factory, refTable: refTable, settings: settings);
                    log.Log.Add(createLog);

                    if (createLog.Status == LogStatus.Processed)
                    {
                        addLog = AddCellToTable(
                            creationLog: createLog
                            , key: key
                            , refTable: ref refTable
                            , overrideKey: overrideKey);
                        log.Log.Add(addLog);
                    }
                }
                else
                {
                    createLog = Logger.Log(cell: null, action: CellAction.Create);
                    createLog.Status = LogStatus.Failed;
                    createLog.Info = key + ":" + factory.Name;
                }
            }

            return log;
        }

        protected ITable CreateNewTable(object target)
        {
            ITable ret;
            if (target != null)
                ret = new TargetTable(target);
            else
                ret = new TableList();
            return ret;
        }

        private TableLog BuildCells(ref ITable refTable, ITable settings)
        {
            TableLog log;
            object target = null;
            if(refTable == null)
            {
                if (settings is ITargetTable tTable)
                    target = tTable.GetTarget();
                refTable = CreateNewTable(target);
            }

            if(CellFactories?.Count() > 0)
            {
                log = AddAllCells(
                    factories: CellFactories
                    , refTable: refTable
                    , settings: settings
                    , overrideKey: OverrideKeys);
            }
            else
            {
                log = Logger.Log(table: refTable, settings: settings);
                log.Status = LogStatus.Failed;
                log.Info = "Словарь фабрики пуст";
            }

            return log;
        }
    }







}
