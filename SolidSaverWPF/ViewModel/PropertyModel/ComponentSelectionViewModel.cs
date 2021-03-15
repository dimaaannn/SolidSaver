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
        #region DeleteSelectedItemsCommand
        private ICommand _DeleteSelectedItemsCommand;

        public ICommand DeleteSelectedItemsCommand => _DeleteSelectedItemsCommand ?? (
            _DeleteSelectedItemsCommand = new RelayCommand<IList>(DeleteSelectedItemsExecute, DeleteSelectedItemsCanExecute));



        private bool DeleteSelectedItemsCanExecute(IList obj) => obj?.Count > 0;

        private void DeleteSelectedItemsExecute(IList obj)
        {
            foreach (IComponentControl selectedComp in obj)
            {
                SelectedComponentList.Remove(selectedComp);
            }
        } 
        #endregion

        private ObservableCollection<IComponentControl> _SelectedComponentList;
        public ObservableCollection<IComponentControl> SelectedComponentList
        {
            get => _SelectedComponentList;
            set => Set(ref _SelectedComponentList, value);
        }
    }
}
