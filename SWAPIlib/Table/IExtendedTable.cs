using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SWAPIlib.Table
{
    /// <summary>
    /// Расширенный функционал таблиц с поддержкой объекта привязки
    /// </summary>
    public interface IExtendedTable : ITargetTable
    {
        ITargetWrapper Target { get; set; }

    }

    
    public class ExtendedTable : BaseTable, IExtendedTable
    {
        public ITargetWrapper Target { get; set; }

        public object GetTarget() => Target?.GetTarget();
    }
}
