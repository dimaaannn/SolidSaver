using SWAPIlib.Property.PropertyBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.PropertyBase
{
    public abstract class TargetBase : ITarget
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public TargetBase()
        {
            Logger.Trace("TargetBase launch constructor");
        }
        protected string targetName;
        protected string targetInfo;

        public virtual TargetType TargetType { get; protected set; }

        public virtual string TargetName { get => targetName; protected set => targetName = value; }

        public virtual string TargetInfo { get => targetInfo; set => targetInfo = value; }

        public IPropertySettings Settings { get; set; }

        public abstract object GetTarget();
    }

}

namespace SWAPIlib.Property 
{ 

    /// <summary>
    /// Типизированный класс цели
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Target<T> : TargetBase, ITarget<T>
    {
        private readonly T target;

        public Target(T target, string name, TargetType type = TargetType.None) : base()
        {
            this.target = target;
            TargetName = name;
            TargetType = type;
            Logger.Trace("PropTarget created with objtype: {objtype}, name: {name} type: {type}", typeof(Type).Name, TargetName, TargetType);
        }

        T ITarget<T>.Target => target;
        public T TTarget => target;

        public override object GetTarget() => target;
    }
}
