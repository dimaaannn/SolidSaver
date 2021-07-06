using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.TaskUnits
{
    public interface ICellLogger
    {
        CellLog Log(ICell cell, ITable settings = null);
        CellLog Log(ICell cell, CellAction action, LogStatus status);
        TableLog Log(ITable table, ITable settings);
    }


    public class SimpleCellLogger<T> : ICellLogger
    {
        Type ClassType = typeof(T);
        public CellLog Log(ICell cell, ITable settings = null)
        {
            CellLog log = new CellLog();
            log.Time = DateTime.Now;
            log.TaskType = ClassType;
            log.Cell = cell;
            log.PrevValue = cell.Text;
            log.Status = LogStatus.None;

            return log;
        }

        public TableLog Log(ITable table, ITable settings)
        {
            TableLog log = new TableLog();
            log.TableTaskType = ClassType;
            log.Status = LogStatus.None;

            return log;
        }

        public CellLog Log(ICell cell, CellAction action, LogStatus status)
        {
            CellLog log = new CellLog();
            log.Time = DateTime.Now;
            log.TaskType = ClassType;
            log.Cell = cell;
            log.Action = action;
            log.Status = status;


            return log;
        }


    }
    public enum LogStatus
    {
        None,
        Processed,
        Passed,
        Failed
    }

    public enum CellAction
    {
        None,
        Create,
        Add,
        Remove
    }

    public struct TableLog
    {
        private LogStatus status;
        public TableLog(Type taskType)
        {
            TableTaskType = taskType;
            status = LogStatus.None;
            Log = new List<CellLog>();
            Info = null;
        }
        public LogStatus Status { 
            get => status; 
            set => status = value; }
        public Type TableTaskType;
        public List<CellLog> Log;
        public string Info;

        private LogStatus GetStatus()
        {
            if (Log.Count == 0)
                return LogStatus.None;
            if(Log.All(l => l.Status == LogStatus.Processed))
                return LogStatus.Processed;
            if (Log.Any(l => l.Status == LogStatus.Failed))
                return LogStatus.Failed;
            return LogStatus.Passed;
        }

        public void Add(CellLog log)
        {
            Log.Add(log);
        }
        public void Add(TableLog tLog)
        {
            Log.AddRange(tLog.Log);
        }

        public static TableLog New => new TableLog { Log = new List<CellLog>() };
    }

    public struct CellLog
    {
        public CellLog(Type taskType)
        {
            Time = DateTime.Now;
            Status = LogStatus.None;
            Action = CellAction.None;
            Cell = null;
            TaskType = taskType;
            PrevValue = null;
            NewValue = null;
            Info = null;
        }
        public DateTime Time;
        
        public LogStatus Status;
        public CellAction Action;
        public ICell Cell;
        public Type TaskType;
        public string PrevValue;
        public string NewValue;
        public string Info;

        public static implicit operator bool(CellLog log) => log.Status == LogStatus.Processed;
        public static CellLog New => new CellLog { Time = DateTime.Now };
    }

}
