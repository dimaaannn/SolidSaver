using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SWAPIlib.Property;
using SWAPIlib.Table;

namespace SWAPIlib.Table.SWProp
{

    public enum SWPropertyNames
    {
        ActiveConfigName
    }

    public class ActiveConfigNameCell : PropertyCellBase
    {
        public ActiveConfigNameCell(ITargetTable refTable) : base(refTable)
        {
            Name = SWPropertyNames.ActiveConfigName.ToString();
            Info = "Имя активной конфигурации";
            AutoUpdate = true;
        }

        public override string Name { get; }
        public override string Info { get; }

        protected static bool GetConfigName(ITargetTable targetTable, out string result)
        {
            object target = targetTable.GetTarget();
            bool ret = false;


            bool test = target is ModelDoc2;

            switch (target)
            {
                case Component2 component:
                    result = SWAPIlib.ComConn.Proxy.ComponentProxy.RefConfigName(component);
                    ret = true;
                    break;

                case ModelDoc2 modelDoc:
                    ret = true;
                    result = SWAPIlib.ComConn.Proxy.ModelConfigProxy.GetActiveConfName(modelDoc);
                    break;
                default:
                    throw new ArgumentException("ActiveConfigNameCell: Некорректный объект цели");
            }

            return ret;
        }

        public override bool Update()
        {
            string result;
            bool ret = GetConfigName(RefTable as ITargetTable, out result);
            Text = result;
            return ret;
        }
    }
}
