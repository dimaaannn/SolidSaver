using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.Data;
using SolidSaverWPF.MessagesType;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.Controller;
using SWAPIlib.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel
{
    public class PartListViewModel : ViewModelBase
    {
        public PartListViewModel()
        {
            MainModel.MainModelChanged += MainModelChanged;
            MessengerInstance.Register<SelectionMessage<object>>(this, ComponentSelected);

        }

        private void ComponentSelected(SelectionMessage<object> obj)
        {
            if(obj.Selection is IComponentControl comp)
            {
                SelectedItemName = comp.Title;
            }
        }

        #region PartListCollection
        private ObservableCollection<IComponentControl> partList;

        public ObservableCollection<IComponentControl> PartList
        {
            get => partList; set => Set(ref partList, value);
        }
        #endregion

        private string selectedItemName;
        public string SelectedItemName { get => selectedItemName; set => Set(ref selectedItemName , value); }

        /// <summary>
        /// Главная модель обновлена
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void MainModelChanged(object o, EventArgs e) => PartList = MainModel.MainModelControl.RootComponents;

    }

    class TreeViewDependency : DependencyObject
    {
        //ICollectionView
    }
}
