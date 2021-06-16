using SWAPIlib.Task;
using System;
using System.Collections;
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
            CheckTarget = CheckTargetPrivate;
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

        public CheckTableDelegate CheckTarget { get; protected set; }
        protected static object GetTargetObject(ITable refTable, ITable settings)
        {
            object ret = null;
            if (settings is ITargetTable tTable)
                ret = tTable.GetTarget();
            else if (refTable is ITargetTable tTable2)
                ret = tTable2.GetTarget();

            return ret;
        }
        protected virtual bool CheckTargetPrivate(ITable refTable, ITable settings)
        {
            return true;
        }

        public abstract string Name { get; }
        public abstract string Info { get; }


    }


    /// <summary>
    /// Создание новой таблицы на основе зависимой
    /// Выборка настроек из зависимой таблицы по иерархии
    /// Сначала просматривается внутренний словарь, если значение null - 
    /// добавляется значение из refTable
    /// Добавление значения к ключу перекрывает refTable
    /// </summary>
    class SettingsTable : ITargetTable
    {
        private ITable _refTable;
        private TargetTable _settingsTable;

        public SettingsTable(ITable refTable)
        {
            _refTable = refTable;
            object target = null;
            if (_refTable is ITargetTable tTable)
                target = tTable.GetTarget();
            _settingsTable = new TargetTable(target) { Name = _refTable.Name };
        }

        public string Name => _settingsTable.Name;

        public void Add(string cellKey, ICell cell, bool replaceVal = true)
        {
            _settingsTable.Add(cellKey, cell, replaceVal);
        }
        public void CopyTo(ITable other, bool overrideKey)
        {
            _settingsTable.CopyTo(other, overrideKey);
        }

        public ICell GetCell(string cellKey)
        {
            ICell ret = _settingsTable.GetCell(cellKey);
            if (ret == null)
            {
                ret = _refTable.GetCell(cellKey);
                if(ret != null)
                {
                    _settingsTable.Add(cellKey: cellKey, cell: ret, replaceVal: true);
                }
            }
            return ret;
        }


        public object GetTarget()
        {
            object ret = _settingsTable.GetTarget();
            if(ret == null && _refTable is ITargetTable tTable)
            {
                ret = tTable.GetTarget();
            }
            return ret;
        }

        public IEnumerator<KeyValuePair<string, ICell>> GetEnumerator() => _settingsTable.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}
