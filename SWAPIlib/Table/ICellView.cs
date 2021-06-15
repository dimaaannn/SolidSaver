using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace SWAPIlib.Table
{
    public interface ICellView : INotifyPropertyChanged
    {
        string Name { get; }
        string Info { get; }
        IEnumerable<ICellView> SettingsList { get; }

        bool IsWritable { get; }
        bool IsReferenced { get; }
        bool IsTargeted { get; }
        bool IsNotSaved { get; }

        string Text { get; }
        string TempText { get; set; }

        void Write();
        void Update();

    }

    public class CellView : ICellView
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        ICell _cell;
        private string _name;

        public CellView(ICell cell)
        {
            _cell = cell;
            _cell.PropertyChanged += _cell_PropertyChanged;
        }

        private void _cell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public string Name { get => GetName(); set { OnPropertyChanged(); _name = value; } }
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
            IEnumerable<ICellView> ret = null;
            if (_cell is IPropertyCell pCell)
            {
                ret = pCell.Settings.Select(keyval => new CellView(keyval.Value) { Name = keyval.Key });
            }
            return ret;
        }

        public void Write()
        {
            if(_cell is IWritableCell wCell && IsNotSaved)
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
        public string Text=> _cell.Text; 

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
                if(_cell is IWritableCell wCell)
                {
                    wCell.TempText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNotSaved => TempText != null;
    }


}
