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

        string Text { get; }
        string TempValue { get; set; }

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

        public bool IsWritable => throw new System.NotImplementedException();

        public bool IsReferenced => throw new System.NotImplementedException();

        public bool IsTargeted => throw new System.NotImplementedException();

        public string Text => throw new System.NotImplementedException();

        public string TempValue { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    }


}
