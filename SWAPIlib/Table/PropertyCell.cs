using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table
{
    public abstract class PropertyCellBase : BaseCell, IPropertyCell
    {
        private string name;
        private IReferencedCell settings;
        private ITable refTable;
        private bool autoUpdate;

        public bool AutoUpdate { get => autoUpdate; set => autoUpdate = value; }

        protected PropertyCellBase(string name, ITargetTable refTable)
        {
            this.name = name;
            this.refTable = refTable;
        }

        public virtual IReferencedCell Settings { get => settings; set { OnPropertyChanged(); settings = value; } }

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
