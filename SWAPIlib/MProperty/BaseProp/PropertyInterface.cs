using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.MProperty.Getters;

namespace SWAPIlib.MProperty
{
    public delegate void PropertyUpdate();
    public delegate bool PropertyWrite(string s);

    public interface IProperty : ICloneable
    {
        bool IsValid { get; }
        bool IsModifyed { get; }
        string PropertyName { get; }
        string TargetName { get; }
        string Value { get; set; }
        IPropView ViewData { get; }

        void Update();
        void ClearValue();
        bool WriteValue(string s);
        bool WriteValue();
        IDataEntity GetTarget();
        IPropGetter GetGetter();
        bool SetTarget(IDataEntity ent);
        bool SetTarget(IPropGetter ent);
    }

    public interface IProperty<TPropGetter, out TBinder, TDataEntity> : IProperty
    {
        TPropGetter PropGetter { get; set; }
        TBinder Binder { get; }
        TDataEntity Entity { get; set; }
        bool SetTarget(TDataEntity ent);
        //bool SetTarget(IPropGetter binder);

    }

    public interface IPropertyModel : 
        IProperty< IPropGetter<IModelBinder>, IModelBinder, IModelEntity>
    {
        
    }

}
