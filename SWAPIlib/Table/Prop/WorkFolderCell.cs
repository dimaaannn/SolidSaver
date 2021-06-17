using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.Prop
{
    public class WorkFolderCell : PropertyCellBase
    {
        public WorkFolderCell() :base(null)
        {
            SWAPIlib.Global.GlobalOptions.WorkFolderChanged += GlobalOptions_WorkFolderChanged;
        }

        public override string Name => ModelPropertyNames.WorkFolder.ToString();

        public override string Info => "Путь к рабочей папке";

        public override bool Update()
        {
            Text = SWAPIlib.Global.GlobalOptions.WorkFolder;
            return !string.IsNullOrEmpty(Text);
        }

        private void GlobalOptions_WorkFolderChanged(object sender, EventArgs e)
        {
            Update();
        }
    }
}
