using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SWAPIlib.Controller;
using SWAPIlib.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel.PropertyModel
{
    public class ComponentSelectionViewModel : ViewModelBase
    {
        public ComponentSelectionViewModel()
        {
            MainModel.MainModelChanged += MainModel_MainModelChanged;
        }

        private void MainModel_MainModelChanged(object sender, EventArgs e)
        {
            SelectedComponentList = MainModel.SelectionList;
        }

        #region DeleteSelectedItemsCommand
        private ICommand _DeleteSelectedItemsCommand;

        public ICommand DeleteSelectedItemsCommand => _DeleteSelectedItemsCommand ?? (
            _DeleteSelectedItemsCommand = new RelayCommand(DeleteSelectedItemsExecute, DeleteSelectedItemsCanExecute));



        private bool DeleteSelectedItemsCanExecute() =>  UserSelection >= 0;

        private void DeleteSelectedItemsExecute()
        {
            SelectedComponentList[UserSelection].IsSelected = false;
        }
        #endregion

        private ObservableCollection<IComponentControl> _SelectedComponentList;
        public ObservableCollection<IComponentControl> SelectedComponentList
        {
            get => _SelectedComponentList;
            set => Set(ref _SelectedComponentList, value);
        }


        public int UserSelection { get; set; }
    }
}
