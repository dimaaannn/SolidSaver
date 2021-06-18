using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidSaverWPF.ViewModel.Table
{
    public class MockCell : PropertyCellBase, IWritableCell
    {
        private string tempText;

        public MockCell() : base(new TargetTable<string>("target!"))
        {
            Settings = new TableList();
            Settings.Add("key", new TextCell("textCell1"));
            Settings.Add("key1", new TextCell("textCell2"));
            Settings.Add("key3", new TextCell("textCell3"));

            RefTable.Add("refTableKey1", new TextCell("refTableCell1"));
            RefTable.Add("refTableKey2", new TextCell("refTableCell2"));
            RefTable.Add("refTableKey3", new TextCell("refTableCell3"));
        }
        public override string Name => "CellName";

        public override string Info => "Some long info with long string reference";

        public string TempText { get => tempText; set { OnPropertyChanged(); tempText = value; } }

        public override bool Update()
        {
            Text = "defaultData";
            TempText = null;
            return true;
        }

        public bool WriteValue()
        {
            Text = TempText;
            TempText = null;
            return string.IsNullOrEmpty(Text);
        }
    }


    /// <summary>
    /// Визуальное представление свойств
    /// </summary>
    public class CellViewModel : ViewModelBase, ICellView
    {
        public CellViewModel()
        {
            if (IsInDesignMode)
            {
                _cell = new MockCell();
            }
            BGColorBrush = new SolidColorBrush(Colors.LightGray);
        }

        ICell _cell;
        private string _name;

        public CellViewModel(ICell cell)
        {
            _cell = cell;
            if (_cell != null)
            {
                _cell.PropertyChanged += _cell_PropertyChanged1;
            }
        }

        public bool IsWritable => _cell is IWritableCell;
        public bool IsReferenced => _cell is IReferencedCell;
        public bool IsTargeted => _cell is IPropertyCell;
        public bool IsNotSaved => TempText != null;


        public string Text => _cell?.Text; //ReadOnly!
        public string Name { get => GetName(); set => Set(ref _name, value); }
        private string GetName()
        {
            if (_name == null && _cell is IPropertyCell pCell)
            {
                _name = pCell.Name;
            }
            return _name;
        }
        public string Info => GetInfo();
        private string GetInfo()
        {
            if (_cell is IPropertyCell pCell)
                return pCell.Info;
            else
                return null;
        }
        public string TempText
        {
            get
            {
                if (_cell is IWritableCell wCell)
                {
                    return wCell.TempText;
                }
                else return null;
            }
            set
            {
                if (_cell is IWritableCell wCell)
                {
                    wCell.TempText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public IEnumerable<ICellView> SettingsList => GetSettings();
        private IEnumerable<ICellView> GetSettings()
        {
            IEnumerable<ICellView> ret = Enumerable.Empty<ICellView>();
            if (_cell is IPropertyCell pCell)
            {
                ret = pCell.Settings.Select(keyval => new CellViewModel(keyval.Value) { Name = keyval.Key });
            }
            return ret;
        }

        private void _cell_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }

        public void Write()
        {
            if (_cell is IWritableCell wCell && IsNotSaved)
            {
                wCell.WriteValue();
            }
        }

        public void Update()
        {
            _cell.Update();
        }

        private ICommand updateProp;
        private bool UpdatePropCanExecute() => _cell != null;
        public ICommand UpdatePropCommand => updateProp ?? (
            updateProp = new RelayCommand(Update, UpdatePropCanExecute));

        private ICommand writeProp;
        private bool WriteCanExecute() => IsWritable && IsNotSaved;
        public ICommand WriteCommand => writeProp ?? (
            writeProp = new RelayCommand(Write, WriteCanExecute));

        private Brush bGColorBrush;
        public Brush BGColorBrush { get => bGColorBrush; set => Set(ref bGColorBrush, value); }

    }



    public class TableViewModel : ViewModelBase
    {
        private ITable _table;
        private string _tableName;


        public TableViewModel(ITable table)
        {
            _table = table;
            Properties = new ObservableCollection<ICellView>(GetCells(_table));
            Name = _table.Name;
        }

        public ITable Table => _table;

        public string Name { get => _tableName; set => Set(ref _tableName, value); }
        public ObservableCollection<ICellView> Properties { get; }

        public bool IsReferencedTable => _table is ITargetTable;

        public string TargetName => "Не реализовано";


        protected IEnumerable<ICellView> GetCells(ITable table)
        {
            if (table.Count() > 0)
            {
                return  table.Select(keyval => new CellViewModel(keyval.Value) { Name = keyval.Key });
            }
            else
                return Enumerable.Empty<ICellView>();
        }
    }
}
