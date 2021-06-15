using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table
{
    public interface IPropertySheet : INotifyPropertyChanged
    {
        ObservableCollection<ICellView> Properties { get; }
        ObservableCollection<ITableView> TableViews { get; }
        ITable GetTable(string tableName);

        void UpdateProperties();
        void WriteProperties();
    }

    public class PropertySheet : IPropertySheet
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public PropertySheet()
        {
            Properties = new ObservableCollection<ICellView>();
            TableViews = new ObservableCollection<ITableView>();
            Tables = new ObservableCollection<ITable>();
            Tables.CollectionChanged += Tables_CollectionChanged;
        }

        private void Tables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                AddTableToView(e.NewItems);
            }
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                RemoveTablesFromView(e.OldItems);
            }

            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                TableViews.Clear();
            }

            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                RemoveTablesFromView(e.OldItems);
                AddTableToView(e.NewItems);
            }
        }

        private void RemoveTablesFromView(System.Collections.IList tables)
        {
            foreach (var tableView in TableViews)
            {
                if (tables.Contains(tableView.Table))
                {
                    TableViews.Remove(tableView);
                }
            }
        }
        private void AddTableToView(System.Collections.IList tables)
        {
            foreach (ITable table in tables)
            {
                TableViews.Add(new TableView(table));
            }
        }

        public ObservableCollection<ICellView> Properties { get; }
        public ObservableCollection<ITableView> TableViews { get; }

        protected ObservableCollection<ITable> Tables { get; }


        public ITable GetTable(string tableName)
        {
            return Tables.Where(table => table.Name == tableName).FirstOrDefault();
        }

        public void UpdateProperties()
        {
            foreach (var prop in Properties)
            {
                prop.Update();
            }
        }

        public void WriteProperties()
        {
            var changedProp = Properties.Where(
                    prp => (prp.IsWritable && prp.IsNotSaved)
                );
            foreach (var prop in changedProp)
            {
                prop.Write();
            }
        }
    }
}
