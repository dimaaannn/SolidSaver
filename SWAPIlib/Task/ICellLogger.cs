using SWAPIlib.Table;
using System;
using System.Collections.Generic;

namespace SWAPIlib.Task
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
        public Type TableTaskType;
        public LogStatus Status;
        public List<CellLog> Log;
        public string Info;
    }

    public struct CellLog
    {
        public DateTime Time;
        
        public LogStatus Status;
        public CellAction Action;
        public ICell Cell;
        public Type TaskType;
        public string PrevValue;
        public string NewValue;
        public string Info;
    }

}
