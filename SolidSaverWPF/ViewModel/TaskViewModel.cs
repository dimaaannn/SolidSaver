﻿using GalaSoft.MvvmLight;
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
            var ret = new List<IExtendedTable>();

            foreach (var component in SWAPIlib.Global.MainModel.SelectionList)
            {
                var target = new TargetWrapper(component.Appmodel);
                ret.Add( new ExtendedTable { Target = target });
            }

            return ret.ToArray();
        }


        public void LoadSelection()
        {
            TableView.Clear();

            var tables = GetSelectedModels();


            SWAPIlib.TaskUnits.Actions.SaveSheetMetalList saveSheetAction = new SWAPIlib.TaskUnits.Actions.SaveSheetMetalList();

            List<TableLog> logList = new List<TableLog>();

            foreach (var table in tables)
            {
                logList.AddRange(saveSheetAction.Proceed(table));
            }


            var showedKeys = new HashSet<string>
            {
                ModelEntities.ConfigName.ToString(),
                ModelPropertyNames.SaveSheetMetal.ToString(),
                "Наименование",
                "Обозначение",
                "dxfFolder"
            };

            try
            {
                //var viewModels = GetViewModel(tables, showedKeys);
                var viewModels = GetViewModel(tables, null);
                foreach (var vm in viewModels)
                {
                    TableView.Add(vm);
                }

            }
            catch (Exception)
            {
                throw;
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
