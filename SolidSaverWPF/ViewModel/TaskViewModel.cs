using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.ViewModel.Table;
using SWAPIlib.Table;
using SWAPIlib.Table.Prop;
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
            var factoryTemplate = new CellFactoryTemplate(); //Источник шаблонов для ячеек

            var getActiveConfigName = new CellFactory(
                factoryTemplate, 
                ModelPropertyNames.ActiveConfigName); 

            var getFileName = new CellFactory(factoryTemplate, ModelPropertyNames.FilePath);

            var workFolderPath = new CellFactory(factoryTemplate, ModelPropertyNames.WorkFolder);

            var textBuilderSettings = TextBuilderCell.BuildSettings(
                (reftable) =>
                {
                    string workFolder = reftable.GetCell(workFolderPath.CellProvider.Key).ToString();
                    string filePath = reftable.GetCell(getFileName.CellProvider.Key).ToString();
                    string savingFileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                    return System.IO.Path.Combine(workFolder, savingFileName);
                });
            
            ITable settings = new TableList { { TextBuilderCell.SETTINGS_KEY, textBuilderSettings, true } };

            var textBuilder = new CellFactory(factoryTemplate, ModelPropertyNames.TextBuilder);

            foreach (var tTable in TableView)
            {
                var table = tTable.Table;

                getActiveConfigName.Proceed(ref table, null);
                getFileName.Proceed(ref table, null);
                workFolderPath.Proceed(ref table, null);

                textBuilder.Proceed(ref table, settings);

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
