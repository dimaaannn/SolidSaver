using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.ViewModel.Table;
using SWAPIlib.Table;
using SWAPIlib.Table.Prop;
using SWAPIlib.TaskUnits;
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


        private TableViewModel currentTableView;
        TableViewModel CurrentTableView { get => currentTableView; set => Set(ref currentTableView, value); }

        private static ICellFactoryTemplate cellFactoryTemplate = new CellFactoryTemplate();

        public void WriteVisibleCells()
        {
            foreach (var table in TableView)
            {
                table.WriteAll();
            }
        }

        public void UpdateVisibleCells()
        {
            foreach (var table in TableView)
            {
                table.UpdateAll();
            }
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

        public ITable GetGlobalTable()
        {
            ITable globalTable = null;


            var workFolderPath = new CellFactory(cellFactoryTemplate, ModelPropertyNames.WorkFolder);
            var workFolderCell = workFolderPath.Proceed(ref globalTable, null).Log.First();

            var dxfFolderName = new TextCell("Развёртки");
            globalTable.Add("dxfFolder", dxfFolderName);

            return globalTable;
        }


        public List<ICellFactory> GetBuildTemplates()
        {
            var ret = new List<ICellFactory>();

            ret.Add(new CellFactory(cellFactoryTemplate, ModelPropertyNames.FileName));
            ret.Add(new CellFactory(
                cellFactoryTemplate,
                ModelPropertyNames.ActiveConfigName));

            return ret;

        }

        public void LoadSelection()
        {
            TableView.Clear();

            var tables = GetSelectedModels();


            #region SettingsTable
            var savingPathBuilder = new CellFactory(cellFactoryTemplate, 
                ModelPropertyNames.TextBuilder);
            savingPathBuilder.CellProvider.Key = ModelEntities.FilePath.ToString();

            var textBuilderSavingPathSettings = TextBuilderCell.BuildSettings(
            (reftable) =>
            {
                string workFolder = reftable.GetCell(ModelEntities.Folder.ToString()).ToString();
                string filePath = reftable.GetCell(ModelEntities.FileName.ToString()).ToString();
                string partFileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                string subFolder = reftable.GetCell("dxfFolder").ToString();
                string snomination = reftable.GetCell("Наименование").ToString();
                string sdesignation = reftable.GetCell("Обозначение").ToString();

                string savingFileName = $"{snomination}-{sdesignation}_{partFileName}";

                //Replace invalid chars in fileName - create new interface
                savingFileName = string.Join("_", 
                    savingFileName.Split(
                        System.IO.Path.GetInvalidFileNameChars()));

                return System.IO.Path.Combine(workFolder, subFolder, savingFileName);
            });
            ITable savingPathSettings = new TableList { { TextBuilderCell.SETTINGS_KEY, textBuilderSavingPathSettings, true } };


            var fileNameViewBuilder = new CellFactory(cellFactoryTemplate, 
                ModelPropertyNames.TextBuilder);
            fileNameViewBuilder.CellProvider.Key = "PartName";
            var textBuilderFileNameSettings = TextBuilderCell.BuildSettings(
                reftable =>
                {
                    string filePath = reftable.GetCell(ModelEntities.FileName.ToString()).ToString();
                    return System.IO.Path.GetFileName(filePath);
                });
            ITable fileNameSettings = new TableList { { TextBuilderCell.SETTINGS_KEY,
                textBuilderFileNameSettings, true } };

            #endregion

            var globalTable = GetGlobalTable();
            var cellsTemplate = GetBuildTemplates();


            var nomination = new CellFactory(
                cellFactoryTemplate, ModelPropertyNames.UserProperty);
            nomination.CellProvider.Key = "Наименование";
            var nominationSettings = new TableList {
                { ModelEntities.UserPropertyName.ToString(),
                    new TextCell("Обозначение"), false} };

            var designation = new CellFactory(
                cellFactoryTemplate, ModelPropertyNames.UserProperty);
            designation.CellProvider.Key = "Обозначение";
            var designationSettings = new TableList {
                { ModelEntities.UserPropertyName.ToString(),
                    new TextCell("Наименование"), false} };

            var saveSheetMetalFactory = new CellFactory(cellFactoryTemplate, ModelPropertyNames.SaveSheetMetal);

            List<TableLog> logList = new List<TableLog>();

            for (int i = 0; i < tables.Length; i++)
            {
                var table = tables[i];

                globalTable.CopyTo(table, false);  //TODO поместить свойства в настройки, не вызывая ошибку

                var validCells = cellsTemplate.Where(template => template.CheckTable(table, null));


                logList.AddRange(validCells.Select(
                    template => template.Proceed(ref table, null))
                    .ToList());

                nomination.Proceed(ref table, nominationSettings);
                designation.Proceed(ref table, designationSettings);
                
                savingPathBuilder.Proceed(ref table, savingPathSettings);
                fileNameViewBuilder.Proceed(ref table, fileNameSettings);

                if(saveSheetMetalFactory.CheckTable(table, null))
                    saveSheetMetalFactory.Proceed(ref table, null);

                tables[i] = table;
            }

            var showedKeys = new HashSet<string>
            {
                ModelEntities.ConfigName.ToString(),
                ModelPropertyNames.SaveSheetMetal.ToString(),
                "Наименование",
                "Обозначение"
            };


            var viewModels = GetViewModel(tables, showedKeys);
            foreach (var vm in viewModels)
            {
                TableView.Add(vm);
            }
        }

        protected static List<TableViewModel> GetViewModel(IEnumerable<ITable> tables, HashSet<string> cellFilter)
        {
            var ret = new List<TableViewModel>();

            bool addFilter = false;
            if (cellFilter?.Count > 0)
                addFilter = true;

            ITable resultTable;
            foreach (var table in tables)
            {
                if (addFilter)
                {
                    resultTable = new TargetTable((table as ITargetTable).GetTarget());

                    var keyVals = from keyval in table
                                  let key = keyval.Key
                                  where cellFilter.Contains(key)
                                  where keyval.Value != null
                                  select keyval;

                    foreach (var keyval in keyVals)
                    {
                        resultTable.Add(keyval.Key, keyval.Value, false);
                    }
                }
                else
                    resultTable = table;

                ret.Add(new TableViewModel(resultTable) { TargetName = table.GetCell("PartName").Text });
            }
            return ret;
        }



        private ICommand getSelectedComponentsCommand;
        private bool GetSelectedComponentsCanExecute() =>
            SWAPIlib.Global.MainModel.SelectionList?.Count() > 0;
        public ICommand GetSelectedComponentsCommand => getSelectedComponentsCommand ?? (
            getSelectedComponentsCommand = new RelayCommand(
                LoadSelection,
                GetSelectedComponentsCanExecute));


        private ICommand writeVisible;
        private bool WriteVisibleCanExecute() => TableView.Count > 0;
        public ICommand WriteVisibleCommand => writeVisible ?? (
            writeVisible = new RelayCommand(WriteVisibleCells, WriteVisibleCanExecute));

        private ICommand updateVisible;
        private bool UpdateVisibleCanExecute() => TableView.Count > 0;
        public ICommand UpdateCommand => updateVisible ?? (
            updateVisible = new RelayCommand(UpdateVisibleCells, UpdateVisibleCanExecute));
    }
}
