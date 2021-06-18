using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SWAPIlib.Table
{
    public interface ITableView : INotifyPropertyChanged
    {
        string Name { get; }
        ObservableCollection<ICellView> Properties { get; }

        bool IsReferencedTable { get; }
        string TargetName { get; }
        ITable Table { get; }
    }

    public class TableView : ITableView
    {
        private ITable _table;
        private string _tableName;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public TableView(ITable table)
        {
            _table = table;
            Properties = new ObservableCollection<ICellView>(GetCells(_table));
            Name = _table.Name;
        }

        public ITable Table => _table;

        public string Name { get => _tableName; set { OnPropertyChanged(); _tableName = value; } }
        public ObservableCollection<ICellView> Properties { get; }

        public bool IsReferencedTable => _table is ITargetTable;

        public string TargetName => "Не реализовано";


        protected IEnumerable<ICellView> GetCells(ITable table)
        {
            if (table.Count() > 0)
            {
                return null;// table.Select(keyval => new CellView(keyval.Value) { Name = keyval.Key });
            }
            else
                return Enumerable.Empty<ICellView>();
        }
    }
}
