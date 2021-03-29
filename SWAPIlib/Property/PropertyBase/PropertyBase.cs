using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.PropertyBase
{
    public abstract class PropertyBase : IProperty
    {
        public PropertyBase(ITargetInteraction target, IPropertyGetter2 propertyGetter)
        {
            Target = target;
            Getter = propertyGetter;
            if(Target.PropertyValue == null)
            {
                Getter.CheckTarget(target); //Create InteractionValue in target
                if(target.PropertyValue == null)
                {
                    string errorMessage = "PropertyBase: Wrong target type";
                    throw new AggregateException(errorMessage);
                    //TODO add logger
                }
            }
        }

        public ITargetInteraction Target { get; protected set; }
        public IPropertyGetter2 Getter { get; protected set; }
        public IInteractionValue PropertyValue => Target.PropertyValue;

        public bool IsModifyed => PropertyValue.UserEdit != null;

        public string Value {
            get
            {
                
                if (PropertyValue.IsReadable == true)
                    return PropertyValue.UserEdit ?? PropertyValue.Current;
                else
                    return "Not avaliable";
            }
            set { 
                if (PropertyValue.IsWritable == true)
                    PropertyValue.UserEdit = value;                
                }
        }
        /// <summary>
        /// Очистить пользовательский ввод
        /// </summary>
        public void ClearSaved()
        {
            if(PropertyValue != null)
                PropertyValue.UserEdit = null ;
        }

        public bool Update()
        {
            throw new NotImplementedException();
        }

        public bool WriteValue()
        {
            throw new NotImplementedException();
        }
    }
}
