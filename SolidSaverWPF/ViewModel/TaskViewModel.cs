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

        public ITable[] GetSelectedModels()
        {
            var ret = new List<ITable>();

            foreach (var component in SWAPIlib.Global.MainModel.SelectionList)
            {
                var target = new TargetWrapper(component.Appmodel);
                ret.Add( new TargetTable(target.GetTarget()));
            }

            return ret.ToArray();
        }

        public void StepOne()
        {
            TableView.Clear();

            var tables = GetSelectedModels();

            var factoryTemplate = new CellFactoryTemplate(); //Источник шаблонов для ячеек
            var getActiveConfigName = new CellFactory(
                factoryTemplate,
                ModelPropertyNames.ActiveConfigName);

            var getFileName = new CellFactory(factoryTemplate, ModelPropertyNames.FileName);
            var workFolderPath = new CellFactory(factoryTemplate, ModelPropertyNames.WorkFolder);
            var savingPathBuilder = new CellFactory(factoryTemplate, ModelPropertyNames.TextBuilder);
            savingPathBuilder.CellProvider.Key = ModelEntities.FilePath.ToString();

            var fileNameViewBuilder = new CellFactory(factoryTemplate, ModelPropertyNames.TextBuilder);
            fileNameViewBuilder.CellProvider.Key = "PartName";

            var saveSheetMetalProp = new CellFactory(factoryTemplate, ModelPropertyNames.SaveSheetMetal);


            #region SettingsTable
            var textBuilderSavingPathSettings = TextBuilderCell.BuildSettings(
            (reftable) =>
            {
                string workFolder = reftable.GetCell(workFolderPath.CellProvider.Key).ToString();
                string filePath = reftable.GetCell(getFileName.CellProvider.Key).ToString();
                string savingFileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                return System.IO.Path.Combine(workFolder, savingFileName);
            });

            var textBuilderFileNameSettings = TextBuilderCell.BuildSettings(
                reftable =>
                {
                    string filePath = reftable.GetCell(getFileName.CellProvider.Key).ToString();
                    return System.IO.Path.GetFileName(filePath);
                });



            ITable savingPathSettings = new TableList { { TextBuilderCell.SETTINGS_KEY, textBuilderSavingPathSettings, true } };
            

            ITable fileNameSettings = new TableList { { TextBuilderCell.SETTINGS_KEY,
                textBuilderFileNameSettings, true } };

            #endregion


            ITable globalTable = null;
            var workFolderCell = workFolderPath.Proceed(ref globalTable, null).Log.First();

            for (int i = 0; i < tables.Length; i++)
            {
                var table = tables[i];

                globalTable.CopyTo(table, false);

                getFileName.Proceed(ref table, null);
                getActiveConfigName.Proceed(ref table, null);
                
                savingPathBuilder.Proceed(ref table, savingPathSettings);
                fileNameViewBuilder.Proceed(ref table, fileNameSettings);

                //Сохранение листового материала
                if(saveSheetMetalProp.CheckTable(table, null))
                    saveSheetMetalProp.Proceed(ref table, null);

                tables[i] = table;
            }

            var showedKeys = new HashSet<string>
            {
                ModelPropertyNames.SaveSheetMetal.ToString(),
                ModelEntities.FilePath.ToString(),
                ModelEntities.ConfigName.ToString(),
                "PartName",
            };


            TableViewModel resultView;
            foreach (var rtable in tables)
            {
                var tempTable = new TargetTable((rtable as ITargetTable).GetTarget());
                if (tempTable.GetTarget() == null)
                    throw new NullReferenceException("Ошибка в передаче объекта цели");

                var keyFilter = rtable.Where(keyval => showedKeys.Contains(keyval.Key));

                foreach (var keyval in keyFilter)
                {
                    tempTable.Add(keyval.Key, keyval.Value, true);
                }

                var table = rtable;

                resultView = new TableViewModel(tempTable);
                TableView.Add(resultView);
            }
        }




        private ICommand getSelectedComponentsCommand;
        private bool GetSelectedComponentsCanExecute() =>
            SWAPIlib.Global.MainModel.SelectionList?.Count() > 0;
        public ICommand GetSelectedComponentsCommand => getSelectedComponentsCommand ?? (
            getSelectedComponentsCommand = new RelayCommand(
                StepOne,
                GetSelectedComponentsCanExecute));

        private ICommand proceedCommand;
        private TableViewModel currentTableView;

        private bool ProceedCanExecute() => false;
        public ICommand ProceedCommand => proceedCommand ?? (
            proceedCommand = new RelayCommand(Proceed, ProceedCanExecute));
    }
}
