using SWAPIlib.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SWAPIlib.Table
{
    /// <summary>
    /// Расширенный функционал таблиц с поддержкой объекта привязки
    /// </summary>
    public interface IExtendedTable :  ITargetTable
    {
        ITarget2 Target { get; set; }

    }

    
    public class ExtendedTable : BaseTable, IExtendedTable
    {
        public ExtendedTable()
        {

        }

        public ExtendedTable(ITarget2 target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public ITarget2 Target { get; set; }

        public string TargetName => Target.TargetName;

        public object GetTarget() => Target?.GetTarget();
    }
}
