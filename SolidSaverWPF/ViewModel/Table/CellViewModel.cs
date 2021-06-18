using GalaSoft.MvvmLight;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSaverWPF.ViewModel.Table
{
    /// <summary>
    /// Визуальное представление свойств
    /// </summary>
    public class CellView : ViewModelBase, ICellView
    {
        public CellView()
        {
            if (IsInDesignMode)
            {

            }
        }

        ICell _cell;
        private string _name;

        public CellView(ICell cell)
        {
            _cell = cell;
            if (_cell != null)
            {
                _cell.PropertyChanged += _cell_PropertyChanged1;
            }
        }

        private void _cell_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }


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

        public IEnumerable<ICellView> SettingsList => GetSettings();

        private IEnumerable<ICellView> GetSettings()
        {
            IEnumerable<ICellView> ret = Enumerable.Empty<ICellView>();
            if (_cell is IPropertyCell pCell)
            {
                ret = pCell.Settings.Select(keyval => new CellView(keyval.Value) { Name = keyval.Key });
            }
            return ret;
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

        public bool IsWritable => _cell is IWritableCell;
        public bool IsReferenced => _cell is IReferencedCell;
        public bool IsTargeted => _cell is IPropertyCell;
        public string Text => _cell?.Text;

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

        public bool IsNotSaved => TempText != null;
    }
}
