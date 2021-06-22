using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
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
            _name = "staticDesignName";
            if (IsInDesignMode)
            {
                _cell = new MockCell();
                var writableCell = Cell as IWritableCell;
                //writableCell.TempText = "This Is temp Text";
                Name = "PropName";
                BorderColorBrush = new SolidColorBrush(Colors.LightGray);
            }


        }

        ICell _cell;
        public ICell Cell { get => _cell; set { Set(ref _cell, value); } }
        private string _name;
        private void _cell_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
            RaisePropertyChanged("IsNotSaved");
        }

        public CellViewModel(ICell cell)
        {
            _cell = cell;
            if (_cell != null)
            {
                _cell.PropertyChanged += _cell_PropertyChanged1;
            }
        }

        public bool IsReadOnly => (_cell is IWritableCell) == false;
        public bool IsReferenced => _cell is IReferencedCell;
        public bool IsTargeted => _cell is IPropertyCell;
        public bool IsNotSaved => TempText != null;


        public string Text => _cell?.Text; //ReadOnly!
        public string TempText
        {
            get
            {
                if (Cell is IWritableCell wCell)
                {
                    return wCell.TempText;
                }
                else return null;
            }
            set
            {
                if (Cell is IWritableCell wCell)
                {
                    wCell.TempText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name { get => _name; set => Set(ref _name, value); }
        public string Info => GetInfo();
        private string GetInfo()
        {
            if (_cell is IPropertyCell pCell)
                return pCell.Info;
            else
                return null;
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


        public void Write()
        {
            if (_cell is IWritableCell wCell && IsNotSaved)
            {
                wCell.WriteValue();
                //RaisePropertyChanged("TempText");
            }
        }
        public void Update()
        {
            _cell.Update();
        }

        public void CopyTextToTemp()
        {
            if (_cell is IWritableCell wcell)
                wcell.TempText = wcell.Text;
        }

        public void ClearTemp()
        {
            TempText = null;
        }
        private ICommand updateProp;
        private bool UpdatePropCanExecute() => _cell != null;
        public ICommand UpdatePropCommand => updateProp ?? (
            updateProp = new RelayCommand(Update, UpdatePropCanExecute));

        private ICommand writeProp;
        private bool WriteCanExecute() => !IsReadOnly && IsNotSaved;
        public ICommand WriteCommand => writeProp ?? (
            writeProp = new RelayCommand(Write, WriteCanExecute));

        private ICommand copyValToText;
        private bool CopyValToTextCanExecute() => IsReadOnly == false && string.IsNullOrEmpty(Text) == false;
        public ICommand CopyValToTextCommand => copyValToText ?? (
            copyValToText = new RelayCommand(CopyTextToTemp, CopyValToTextCanExecute));

        private ICommand clearTempTextCommand;
        private bool ClearTempTextCommandCanExecute() => _cell is IWritableCell wcell && wcell.TempText != null;
        public ICommand ClearTempTextCommand => clearTempTextCommand ?? (
            clearTempTextCommand = new RelayCommand(ClearTemp, ClearTempTextCommandCanExecute));

        private Brush borderColorBrush = new SolidColorBrush(Colors.Gray);
        public Brush BorderColorBrush { get => borderColorBrush; set => Set(ref borderColorBrush, value); }


    }
}
