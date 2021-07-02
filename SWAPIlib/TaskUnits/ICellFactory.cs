using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    public interface ICellFactory : ITableAction, ITableChecker
    {
        ICellFactoryProvider CellProvider { get; }
    }


    public class CellFactory : TableActionBase, ICellFactory
    {
        protected static ICellLogger logBuilder = new SimpleCellLogger<CellFactory>();

        public CellFactory(ICellFactoryProvider cellProvider) 
        {
            if (cellProvider == null)
                throw new NullReferenceException("CellFactory: cellProvider null reference");
            CellProvider = cellProvider;
        }

        public CellFactory(ICellFactoryTemplate cellFactoryTemplate, ModelPropertyNames modelProperty) : this(cellFactoryTemplate.GetCellProvider(modelProperty)) { }
        public override string Name => CellProvider.Name;
        public ICellFactoryProvider CellProvider { get; set; }

        public CheckTableDelegate CheckTable => CellProvider.CheckTable;

        public override TableLog Proceed(ref ITable refTable, ITable settings)
        {
            TableLog retLog;
            CellLog createLog;
            CellLog addLog;
            string key = CellProvider.Key;
            bool overrideKey = CellProvider.OverrideKey;

            if(refTable == null)
            {
                refTable = CreateNewTable(
                    settings is ITargetTable tTable 
                    ? tTable.GetTarget() 
                    : null);
            }

            createLog = CreateCell(CellProvider, refTable: refTable, settings: settings);
            if(createLog.Status == LogStatus.Processed)
            {
                addLog = AddCellToTable(
                    cell: createLog.Cell
                    , key: key
                    , refTable: refTable
                    , overrideKey: overrideKey);
            }
            else
            {
                addLog = logBuilder.Log(cell: createLog.Cell, action: CellAction.Add, status: LogStatus.Passed);
                addLog.Info = "Некорректное свойство пропущено";
            }

            retLog = logBuilder.Log(table: refTable, settings: settings);
            retLog.Info = CellProvider.Name + " cell factory";
            retLog.Log = new List<CellLog>
            {
                createLog,
                addLog
            };
            retLog.Status = createLog.Status;

            return retLog;

        }


        public static CellLog CreateCell(ICellFactoryProvider cellProvider, ITable refTable, ITable settings = null)
        {
            ICell cell = null;
            bool isValid = cellProvider.CheckTable(refTable: refTable, settings: settings);

            if (isValid)
            {
                cell = cellProvider.GetCell(refTable: refTable, settings: settings);
            }

            CellLog ret = logBuilder.Log(cell, settings);
            ret.Info = cellProvider.Name;
            ret.Status = isValid ? LogStatus.Processed : LogStatus.Failed;

            return ret;
        }

        public static CellLog AddCellToTable(ICell cell, string key, ITable refTable, bool overrideKey = true)
        {
            ICell cellInTable;
            var retLog = logBuilder.Log(cell, CellAction.Add, LogStatus.None);
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
