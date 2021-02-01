using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.MProperty.BaseProp
{
    public delegate void PropertyUpdate();
    public delegate bool PropertyWrite(string s);

    public interface IProperty : ICloneable
    {
        bool IsModifyed { get; }
        string PropertyName { get; }
        string TargetName { get; }
        string Value { get; set; }

        IBinder Binder { get; }
        IDataEntity Entity { get; set; }
        IPropView ViewData { get; }

        PropertyUpdate Update { get; }
        PropertyWrite WriteValue {get;}
    }
}
