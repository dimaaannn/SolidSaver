using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.SWProp
{
    public class FilePathCell : PropertyCellBase
    {
        public FilePathCell(ITargetTable refTable) : base(refTable: refTable)
        {

        }

        public override string Name => ModelPropertyNames.FilePath.ToString();
        public override string Info => "Полный путь к файлу модели";

        public override bool Update()
        {
            bool ret = false;
            object target = GetTarget();

            switch (target)
            {
                case ModelDoc2 model:
                    Text = SWAPIlib.ComConn.Proxy.ModelProxy.GetPathName(model);
                    ret = true;
                    break;
                case Component2 comp:
                    Text = SWAPIlib.ComConn.Proxy.ComponentProxy.GetPathName(comp);
                    ret = true;
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static bool CheckTargetType(ITable refTable, ITable settings) =>
            PropertyCellUtils.TargetIsModel(refTable, settings)
            || PropertyCellUtils.TargetIsComponent(refTable, settings);
    }
}
