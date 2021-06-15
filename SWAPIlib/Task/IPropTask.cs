using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task
{
    public interface ICellTask
    {
        string Name { get; }
        CellLog Proceed(ref ICell cell, ITable settings);
    }

    public abstract class CellTaskBase : ICellTask
    {
        protected static ICellLogger Logger = new SimpleCellLogger();

        public abstract string Name { get; }

        public abstract CellLog Proceed(ref ICell cell, ITable settings);
    }


    public delegate bool CellFactoryDelegate(ITable table, out ICell cell);
    public interface ICellFactory
    {
        Dictionary<string, CellFactoryDelegate> CellBuilders { get; }
    }


    public class CellFactoryBase : ICellFactory
    {
        public Dictionary<string, CellFactoryDelegate> CellBuilders => throw new NotImplementedException();

        public bool BuildTable(ref ITable refTable, ITable settings, bool replaceOld)
        {
            if(refTable == null)
            {
                refTable = new SettingsTable(refTable);
            }

            ICell temp;
            bool isValid;
            foreach (var keyVal in CellBuilders)
            {


                isValid = keyVal.Value(settings, out temp);
                if (isValid)
                {
                    refTable.
                }
            }
        }

        protected  virtual bool BuildCell(CellFactoryDelegate cellBuilder, ITable settings, out ICell cell)
        {
            if(cellBuilder == null)
            {
                throw new ArgumentNullException("CellFactoryBase - null builder");
            }

            bool ret = cellBuilder(settings, out cell);
            return ret;
        }

        protected bool CheckKeyExist(ITable settings, string key)
        {
            var cell = settings.GetCell(key);
            return cell == null;
        }

    }

    public abstract class TableTaskBase : ITableTask
    {
        protected static ITable GetNewTable(bool targeted = false)
        {
            if (targeted)
            {
                return new TargetTable(null);
            }
            else
                return new TableList();
        }
        protected ICellLogger Logger = new SimpleCellLogger();
        public string Name { get; }
        public abstract TableLog Proceed(ref ITable table, ITable settings);
    }


    public class CellFactoryConfigName : ICellFactory
    {
        public string Key { get; set; } = ModelEntities.ConfigName.ToString();

        public Func<ITable, ICell, bool> GetCell { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }




    public class TableTaskAddUP : TableTaskBase
    {
        public ICellFactory UserPropName { get; set; }
        public override TableLog Proceed(ref ITable table, ITable settings)
        {
            var log = Logger.Log<TableTaskAddUP>(table, settings);

            if (settings is ITargetTable tTable)
            {
                log.TableAction = CellAction.Processed;

            }
            else
                log.TableAction = CellAction.Failed;

            return log;
        }

        

    }


    public interface ICellLogger
    {
        CellLog Log<T>(ICell cell, ITable settings = null);
        TableLog Log<T>(ITable table, ITable settings);
    }

    public class SimpleCellLogger : ICellLogger
    {
        public CellLog Log<T>(ICell cell, ITable settings = null)
        {
            CellLog log = new CellLog();
            log.Time = DateTime.Now;
            log.TaskType = typeof(T);
            log.Cell = cell;
            log.PrevValue = cell.Text;
            log.Action = CellAction.None;

            return log;
        }

        public TableLog Log<T>(ITable table, ITable settings)
        {
            TableLog log = new TableLog();
            log.TableTaskType = typeof(T);
            log.TableAction = CellAction.None;

            return log;
        }
    }


    public struct TableLog
    {
        public Type TableTaskType;
        public CellAction TableAction;
        public List<CellLog> Log;
    }

    public struct CellLog
    {
        public DateTime Time;
        public CellAction Action;
        public ICell Cell;
        public Type TaskType;
        public string PrevValue;
        public string NewValue;
    }

    public enum CellAction
    {
        None,
        Processed,
        Passed,
        Failed
    }

}
