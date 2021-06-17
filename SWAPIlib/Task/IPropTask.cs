using SWAPIlib.Table;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task
{
    public delegate TableLog TableActionDelegate(ref ITable refTable, ITable settings);

    public delegate bool CheckTableDelegate(ITable refTable, ITable settings);
    public delegate ICell CellGetterDelegate(ITable refTable, ITable settings);


    public interface ITableChecker
    {
        CheckTableDelegate CheckTable { get; }
    }

    public interface ITableAction
    {
        string Name { get; }
        TableLog Proceed(ref ITable refTable, ITable settings);
    }

    public interface ICellInTableAction
    {
        string Name { get; }
        CheckTableDelegate CheckTable { get; }
        CellGetterDelegate GetCell { get; }
    }

    public interface ICellTask
    {
        string Name { get; }
        CellLog Proceed(ref ICell cell, ITable settings);
    }

    public abstract class CellTaskBase : ICellTask
    {
        protected static ICellLogger Logger = new SimpleCellLogger<CellTaskBase>();

        public abstract string Name { get; }

        public abstract CellLog Proceed(ref ICell cell, ITable settings);
    }




}
