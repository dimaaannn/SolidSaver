using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.SWProp
{
    public abstract class UserPropertyCellBase : PropertyCellBase
    {
        public string PropertyName { get; }
        public UserPropertyCellBase(ITargetTable reftable) : base(reftable)
        {
            Settings = new Table();
            Settings.Add(ConfigNameKey, null);
            Settings.Add(PropNameKey, null);
        }
        public static readonly string PropNameKey = ModelEntities.UserPropertyName.ToString();
        public static readonly string ConfigNameKey = ModelEntities.ConfigName.ToString();

        public string UserPropertyName => Settings?.GetCell(PropNameKey)?.ToString();
        public string ConfigName => Settings?.GetCell(ConfigNameKey)?.ToString();


        public override string Name => ModelPropertyNames.UserProperty.ToString();
        public override string Info => $"Пользовательское свойство {UserPropertyName}";
    }



    public class UserPropertyCell : UserPropertyCellBase
    {
        public UserPropertyCell(ITargetTable reftable) : base(reftable) 
        {

        }

        protected bool GetUserProperty(object target, out string result)
        {
            bool ret = false;
            result = null;
            if(target is ModelDoc2 model
                && !string.IsNullOrEmpty(ConfigName)
                && !string.IsNullOrEmpty(UserPropertyName))
            {
                ret = true;
                result = SWAPIlib.ComConn.Proxy.ModelConfigProxy.GetConfParamValue(
                    swModel: model,
                    configName: ConfigName,
                    fieldName: UserPropertyName);
            }
            return ret;
        }

        public override bool Update()
        {
            string result;
            bool ret = GetUserProperty(Target, out result);
            Text = result;
            return ret;
        }
    }
}
