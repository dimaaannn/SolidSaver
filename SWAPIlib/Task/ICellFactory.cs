using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task
{
    public interface ICellFactory
    {
        string Name { get; }
        ITable SettingsTemplate { get; }
        Type TargetType { get; }
        CheckTargetDelegate CheckTarget { get; set; }
        CellActionDelegate Build { get; set; }
    }



    public class PropFactory : ICellFactory
    {
        public string Name { get; set; }

        public ITable SettingsTemplate { get; set; }
        public Type TargetType { get; set; }
        public CheckTargetDelegate CheckTarget { get; set; } = (x, y) => true;
        public CellActionDelegate Build { get; set; }

    }

    public delegate bool CheckTargetDelegate(ITable refTable, ITable settings);
    public delegate ICell CellActionDelegate(ITable refTable, ITable settings);
}
