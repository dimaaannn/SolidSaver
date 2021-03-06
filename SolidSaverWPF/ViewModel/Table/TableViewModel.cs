﻿using GalaSoft.MvvmLight;
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

        public ITable Table
        {
            get => _table;
            set
            {
                Set(ref _table, value);
                Properties.Clear();
                foreach (var cell in Table)
                {
                    Properties.Add(new CellViewModel(cell.Value) { Name = cell.Key });
                }
            }
        }

        public string Name { get => _tableName; set => Set(ref _tableName, value); }
        public ObservableCollection<CellViewModel> Properties { get; }
        public bool HasNoError { get; set; } = true;

        public bool IsReferencedTable => _table is ITargetTable;

        public string TargetName { get; set; }

        protected IEnumerable<CellViewModel> GetCells(ITable table)
        {
            if (table.Count() > 0)
            {
                return  table.Select(keyval => new CellViewModel(keyval.Value) { Name = keyval.Key });
            }
            else
                return Enumerable.Empty<CellViewModel>();
        }

        public void WriteAll()
        {
            HasNoError = true;
            var notSaved = from propcell in Properties
                           let cell = propcell.Cell
                           where cell is IWritableCell
                           let wcell = cell as IWritableCell
                           where wcell.TempText != null
                           select wcell;
            foreach (var cell in notSaved)
            {
                HasNoError &= cell.WriteValue();
            }
        }

        public void UpdateAll()
        {
            HasNoError = true;
            foreach (var prop in Properties)
            {
                prop.Cell.Update();
            }
        }
    }
}
