using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.ViewModel.Table;
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
        private ObservableCollection<TableViewModel> tableView = new ObservableCollection<TableViewModel>();
        public ObservableCollection<TableViewModel> TableView { get => tableView; set => Set(ref tableView, value); }


        TableViewModel CurrentTableView { get => currentTableView; set => Set(ref currentTableView, value); }

        public void Proceed()
        {
            var factoryTemplate = new CellFactoryTemplate();

            var getActiveConfigName = new CellFactory(factoryTemplate, ModelPropertyNames.ActiveConfigName);
            var getFileName = new CellFactory(factoryTemplate, ModelPropertyNames.FilePath);


            ITable settings = null;
            foreach (var tTable in TableView)
            {
                var table = tTable.Table;

                getActiveConfigName.Proceed(ref table, null);
                getFileName.Proceed(ref table, null);

                tTable.Table = table;
            }
        }

        public void TestSaveDxf()
        {

        }

        public ObservableCollection<TableViewModel> GetTables()
        {
            var ret = new ObservableCollection<TableViewModel>();

            ITable tempTable;
            foreach (var component in SWAPIlib.Global.MainModel.SelectionList)
            {
                var target = new TargetWrapper(component.Appmodel);
                tempTable = new TargetTable(target.GetTarget());
                ret.Add(new TableViewModel(tempTable));
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
        private TableViewModel currentTableView;

        private bool ProceedCanExecute() => TableView.Count > 0;
        public ICommand ProceedCommand => proceedCommand ?? (
            proceedCommand = new RelayCommand(Proceed, ProceedCanExecute));
    }
}
