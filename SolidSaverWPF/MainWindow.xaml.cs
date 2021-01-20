﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SWAPIlib;
using System.Collections.ObjectModel;
using SWAPIlib.PropertyObj;
using SWAPIlib.Controller;
using SWAPIlib.ComConn;
using SWAPIlib.Global;

namespace SolidSaverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Законченные классы
        public SWAPIlib.Global.IMainPartView MainPartView { get; set; }
        public IRootModel MainModel { get; set; }

        public List<IAppComponent> SubComponents { get => MainModel.SubComponents; }
        public AppComponent SelectedModel { get; set; }
        public IPropertyUI PropUI { get; set; }
        public IList<ISwProperty> PropList { get; set; }

        //Tests
        /// <summary>
        /// Выбранные компоненты для загрузки в список свойств
        /// </summary>
        public List<IAppComponent> SelectedComp { get; } = new List<IAppComponent>();
        public KeyValuePair<string, ISwProperty> TestPropPair { get; set; }
        public ObservableCollection<ComponentControl> CompControlList;


        public MainWindow()
        {
            InitializeComponent();
            MainModel = new RootModel();
            this.DataContext = this;

            SwAppControl.Connect();
            MainModel.GetMainModel();
            MainModel.TopLevelOnly = true;
            MainModel.GetSubComponents();

            //Основной список деталей
            MainPartView = new SWAPIlib.Global.MainPartView(MainModel);
            //PartViewList.TreePartView.ItemsSource = MainPartView.RootComponents;
            PartViewList.MainPartView = MainPartView;



            #region Свойства поиска

            //Замена свойств
            PropUI = new PropertyUI();
            //Привязка к ЮзерКонтрол
            PropertyTab.DataContext = PropUI;
            //Тестовый список хранения
            PropUI.ComponentList = SelectedComp;

            //PartsList.ItemsSource = SubComponents;
            #endregion

            CompControlList = new ObservableCollection<ComponentControl>();

           
            //MainPartView.SelectedCompProp
            
            //Run new thread
            //System.Threading.ThreadPool.QueueUserWorkItem(TestPartList.ChangeSelection);
        }




        /// <summary>
        /// Обновить свойства активной детали
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            foreach(var prop in PropList)
            {
                prop.Update();
            }

        }

        /// <summary>
        /// Записать значения свойств активной детали
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonWrite_Click(object sender, RoutedEventArgs e)
        {
            foreach (var prop in PropList)
                prop.WriteValue();
        }

        /// <summary>
        /// Записать значения свойств всех деталей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonWriteAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var comp in SubComponents)
            {
                foreach (var prop in comp.PropList)
                    prop.WriteValue();
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            MainModel.LoadActiveModel();
            MainModel.GetSubComponents();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var comp in MainPartView.RootComponents)
            {
                comp.IsSelected = !comp.IsSelected;
                foreach ( var scomp in comp)
                {
                    scomp.IsSelected = !scomp.IsSelected;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach(var comp in MainPartView.RootComponents)
            {
                comp.SubComponents.Clear();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainPartView.ReloadCompList();
        }

        //private void PartsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    var index = PartsList.SelectedIndex;
        //    if (index >= 0)
        //    {
        //        var currentComp = SubComponents[index];
        //        SelectedComp.Add(currentComp);
        //    }
        //}
    }

    

}
