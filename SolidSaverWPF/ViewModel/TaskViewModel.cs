using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SWAPIlib.Table;
using SWAPIlib.Task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel
{
    public class TaskViewModel : ViewModelBase
    {
        public ObservableCollection<ITableView> TableView { get => tableView; set => Set(ref tableView , value); }
        protected ICellFactory GetFactory()
        {
            ICellFactory ret;
            var factoryTemplate = new CellFactoryTemplate();
            ret = new CellFactory(factoryTemplate, ModelPropertyNames.ActiveConfigName);
            return ret;
        }

        public void Proceed()
        {
            var factory = GetFactory();

            foreach (var tTable in TableView)
            {
                var table = tTable.Table;

                factory.Proceed(ref table, null);

                if (tTable.Table != table)
                    tTable.Table = table;
            }
        }

        public ObservableCollection<ITableView> GetTables()
        {
            var ret = new ObservableCollection<ITableView>();

            ITable tempTable;
            foreach (var component in SWAPIlib.Global.MainModel.SelectionList)
            {
                var target = new TargetWrapper(component.Appmodel);
                tempTable = new TargetTable(target.GetTarget());
                ret.Add(new TableView(tempTable));
            }
            return ret;
        }

        private ICommand getSelectedComponentsCommand;
        private bool GetSelectedComponentsCanExecute() =>
            SWAPIlib.Global.MainModel.SelectionList?.Count() > 0;
        public ICommand GetSelectedComponentsCommand => getSelectedComponentsCommand ?? (
            getSelectedComponentsCommand = new RelayCommand(
                    () => TableView = GetTables(),
                GetSelectedComponentsCanExecute));

        private ICommand proceedCommand;
        private ObservableCollection<ITableView> tableView = new ObservableCollection<ITableView>();

        private bool ProceedCanExecute() => TableView.Count > 0;
        public ICommand ProceedCommand => proceedCommand ?? (
            proceedCommand = new RelayCommand(Proceed, ProceedCanExecute));
    }
}
