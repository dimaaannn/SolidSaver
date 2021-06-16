using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task
{
    public interface ICellFactory : ITableAction
    {
        /// <summary>
        /// Ключ добавления в списки
        /// </summary>
        string Key { get; set; }

        bool OverrideKey { get; set; }
        ICellProvider CellProvider { get; }
    }


    public class CellFactory : ICellFactory
    {
        public CellFactory()
        {
            Proceed = ProceedAction;
            CheckTable = (x, y) => true;
        }

        public CellFactory(ICellProvider cellProvider) : this()
        {
            Name = cellProvider.Name;
            CheckTable = cellProvider.CheckTable;


        }
        protected static ICellLogger Logger = new SimpleCellLogger<CellFactory>();
        public string Name { get; set; }
        public string Key { get; set; }
        public bool OverrideKey { get; set; }

        public TableActionDelegate Proceed { get; }

        public CheckTableDelegate CheckTable { get; set; } 
        public ICellProvider CellProvider { get; set; }

        protected virtual TableLog ProceedAction(ref ITable refTable, ITable settings)
        {
            ICellProvider cellProvider = CellProvider;
            TableLog retLog;
            CellLog createLog;
            CellLog addLog; 
            string key = Name;
            bool overrideKey = OverrideKey;

            if(refTable == null)
            {
                refTable = CreateNewTable(
                    settings is ITargetTable tTable 
                    ? tTable.GetTarget() 
                    : null);
            }

            createLog = CreateCell(cellProvider, refTable: refTable, settings: settings);
            if(createLog.Status == LogStatus.Processed)
            {
                addLog = AddCellToTable(
                    cell: createLog.Cell
                    , key: Name
                    , refTable: refTable
                    , overrideKey: overrideKey);
            }
            else
            {
                addLog = Logger.Log(cell: createLog.Cell, action: CellAction.Add, status: LogStatus.Passed);
                addLog.Info = "Некорректное свойство пропущено";
            }

            retLog = Logger.Log(table: refTable, settings: settings);
            retLog.Info = CellProvider.Name + " cell factory";
            retLog.Log.Add(createLog);
            retLog.Log.Add(addLog);
            retLog.Status = createLog.Status;

            return retLog;

        }


        public static CellLog CreateCell(ICellProvider cellProvider, ITable refTable, ITable settings = null)
        {
            ICell cell = null;
            bool isValid = cellProvider.CheckTable(refTable: refTable, settings: settings);

            if (isValid)
            {
                cell = cellProvider.GetCell(refTable: refTable, settings: settings);
            }

            CellLog ret = Logger.Log(cell, settings);
            ret.Info = cellProvider.Name;
            ret.Status = isValid ? LogStatus.Processed : LogStatus.Failed;

            return ret;
        }

        public static CellLog AddCellToTable(ICell cell, string key, ITable refTable, bool overrideKey = true)
        {
            ICell cellInTable;
            var retLog = Logger.Log(cell, CellAction.Add, LogStatus.None);
            retLog.Info = $"{key}:{refTable.Name}";


            refTable.Add(key, cell, overrideKey);
            cellInTable = refTable.GetCell(key);

            if (cellInTable != cell)
            {
                if (overrideKey)
                    retLog.Status = LogStatus.Passed;
                else
                    retLog.Status = LogStatus.Failed;
            }
            else
                retLog.Status = LogStatus.Processed;

            return retLog;
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
    }

    
}
