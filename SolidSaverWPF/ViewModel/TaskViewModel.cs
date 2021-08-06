using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.ViewModel.Table;
using SWAPIlib.BaseTypes;
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


        public SWAPIlib.TaskCollection.ITableCollection GetSelectedModels()
        {

            var tableProvider = new SWAPIlib.TaskCollection.TableProviderTemp();
            return tableProvider.UserSelectedModels();
        }


        public void LoadSelection()
        {
            TableView.Clear();

            //Получить список выделенных пользователем деталей (где проставлены галочки)
            var userSelectedModels = GetSelectedModels();

            #region Тестовая заготовка для отладки

            //Создать тестовый класс-поставщик действий (использовался в прошлый раз)
            //Сейчас - никак не используется. Просто висит.
            var saveSheetPrevAction = new SWAPIlib.TaskUnits.Actions.SaveSheetMetalList();

            //Создать тестовый класс со всеми зависимостями (Через DI)
            var tableProviderTest = new SWAPIlib.TaskCollection.TableProviderTemp();
            //Получить из этого класса набор действий
            var actionUnit = tableProviderTest.GetActionUnit();
            #endregion

            List<TableLog> logList = new List<TableLog>();

            //Запустить обработку таблиц для каждой из выбранной ранее деталей
            foreach (var modelTable in userSelectedModels)
            {
                actionUnit.Run(modelTable);
            }

            //Создать визуальные представления для каждой из таблиц
            var viewModels = GetViewModel(userSelectedModels);

            //Показать представления пользователю
            foreach (var vm in viewModels)
            {
                TableView.Add(vm);
            }
        }

        /// <summary>
        /// Создать визуальные представления для таблиц с свойствами
        /// </summary>
        /// <param name="tableCollection"></param>
        /// <returns></returns>
        protected static List<TableViewModel> GetViewModel(SWAPIlib.TaskCollection.ITableCollection tableCollection)
        {
            var ret = new List<TableViewModel>();

            foreach (var table in tableCollection)
            {
                ret.Add(new TableViewModel(table) { TargetName = table.TargetName });
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
