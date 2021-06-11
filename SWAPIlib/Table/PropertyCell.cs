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
        public object Target => (RefTable as ITargetTable)?.GetTarget();

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

        public abstract string Name { get; }
        public abstract string Info { get; }
    }



}
