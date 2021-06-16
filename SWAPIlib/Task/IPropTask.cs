using SWAPIlib.Table;
using System;
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
        protected static ICellLogger Logger = new SimpleCellLogger<CellTaskBase>();

        public abstract string Name { get; }

        public abstract CellLog Proceed(ref ICell cell, ITable settings);
    }



    public delegate TableLog TableActionDelegate(ref ITable refTable, ITable settings);

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
            var log = Logger.Log(table, settings);

            if (settings is ITargetTable tTable)
            {
                log.Status = LogStatus.Processed;

            }
            else
                log.Status = LogStatus.Failed;

            return log;
        }

        

    }







}
