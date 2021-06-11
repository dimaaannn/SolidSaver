using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.SWProp
{
    public abstract class UserPropertyCellBase : PropertyCellBase, IWritableCell
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
        private string tempText;

        public string UserPropertyName => Settings?.GetCell(PropNameKey)?.ToString();
        public string ConfigName => Settings?.GetCell(ConfigNameKey)?.ToString();

        public string TempText { get => tempText; set { OnPropertyChanged(); tempText = value; } }

        public override string Name => ModelPropertyNames.UserProperty.ToString();
        public override string Info => $"Пользовательское свойство {UserPropertyName}";

        public abstract bool WriteValue();
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

        protected bool SetUserProperty(object target, string newText)
        {
            bool ret = false;

            if (target is ModelDoc2 model
                && !string.IsNullOrEmpty(ConfigName)
                && !string.IsNullOrEmpty(UserPropertyName))
            {
                ret = SWAPIlib.ComConn.Proxy.ModelConfigProxy.SetConfParam(
                    swModel: model,
                    configName: ConfigName,
                    fieldName: UserPropertyName,
                    fieldVal: newText);
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

        public override bool WriteValue()
        {
            bool ret = false;
            if(TempText != null)
            {
                ret = SetUserProperty(Target, TempText);
            }

            if (ret)
            {
                TempText = null;
                Text = null;
            }

            return ret;
        }
    }
}
