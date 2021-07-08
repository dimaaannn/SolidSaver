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
        public ITarget2 Target { get; set; }

        public object GetTarget() => Target?.GetTarget();
    }
}
