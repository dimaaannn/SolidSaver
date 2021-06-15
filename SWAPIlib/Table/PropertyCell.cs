using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table
{
    public abstract class PropertyCellBase : BaseCell, IPropertyCell
    {
        private ITable settings;
        private ITable refTable;
        private bool autoUpdate = true;

        public bool AutoUpdate { get => autoUpdate; set => autoUpdate = value; }
        public object Target => GetTarget();

        protected PropertyCellBase(ITargetTable refTable)
        {
            this.refTable = refTable;
        }

        public virtual ITable Settings { get => settings; set { OnPropertyChanged(); settings = value; } }

        public ITable RefTable { get => refTable; set { OnPropertyChanged(); refTable = value; } }

        public override string Text
        {
            get
            {
                if (base.Text == null && AutoUpdate)
                {
                    AutoUpdate = Update(); // Предотвратить ошибочные вызовы
                }

                return base.Text;
            }
            protected set => base.Text = value;
        }

        protected ICell GetSettings(string key)
        {
            ICell ret;

            ret = Settings?.GetCell(key);
            if (ret != null)
                return ret;
            ret = RefTable?.GetCell(key);
            if (ret != null)
                return ret;

            return null;
        }
        protected object GetTarget()
        {
            object ret = null;
            
            if(Settings is ITargetTable tTable)
            {
                ret = tTable.GetTarget();
            }
            if(ret == null && RefTable is ITargetTable tTable2)
            {
                ret = tTable2.GetTarget();
            }

            return ret;
        }

        public abstract string Name { get; }
        public abstract string Info { get; }
    }



}
