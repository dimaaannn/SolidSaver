using GalaSoft.MvvmLight;
using SWAPIlib.Table;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SolidSaverWPF.ViewModel.Table
{
    public class TableViewModel : ViewModelBase
    {
        private ITable _table;
        private string _tableName;

        public TableViewModel()
        {
            if (IsInDesignMode)
            {
                var mockCell = new MockCell();
                Properties = new ObservableCollection<CellViewModel>(GetCells(mockCell.RefTable));
                Name = "TestName";
            }
        }

        public TableViewModel(ITable table)
        {
            _table = table;
            Properties = new ObservableCollection<CellViewModel>(GetCells(_table));
            Name = _table.Name;
        }

        public ITable Table => _table;

        public string Name { get => _tableName; set => Set(ref _tableName, value); }
        public ObservableCollection<CellViewModel> Properties { get; }

        public bool IsReferencedTable => _table is ITargetTable;

        public string TargetName => "Не реализовано";


        protected IEnumerable<CellViewModel> GetCells(ITable table)
        {
            if (table.Count() > 0)
            {
                return  table.Select(keyval => new CellViewModel(keyval.Value) { Name = keyval.Key });
            }
            else
                return Enumerable.Empty<CellViewModel>();
        }
    }
}
