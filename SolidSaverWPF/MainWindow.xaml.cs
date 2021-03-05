using System;
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
using SWAPIlib.MProperty;

namespace SolidSaverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Законченные классы

        /// <summary>
        /// Поиск и замена свойств
        /// </summary>
        public IPropertyUI PropUI { get; set; }



        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            
            

           
            //MainPartView.SelectedCompProp
            
            //Run new thread
            //System.Threading.ThreadPool.QueueUserWorkItem(TestPartList.ChangeSelection);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            #region Old loading options
            //MainLinkedModelTemp = MainModel.GetMainModel();
            //this.DataContext = this;

            SwAppControl.Connect();
            //MainLinkedModelTemp.GetMainModel();
            //MainLinkedModelTemp.TopLevelOnly = true;
            //MainLinkedModelTemp.GetSubComponents();

            ////Основной список деталей
            ////Создать класс
            //MainPartview = new SWAPIlib.Global.MainPartControl(MainLinkedModelTemp);
            /////Подключить к списку деталей WPF - Заменить на binding
            ////PartViewList.MainPartView = MainPartview;
            ////Fix binding bug
            ////Bind Main Part list to interface
            //PartViewList.DataContext = MainPartview;
            //PropertyBox.DataContext = MainPartview;
            ////MainPartview.RootComponents
            ////PropertyBox.ItemsSource = MainPartview.SelectedCompProp;

            //var configurator = PropertyView.PropConfigurator.GetConfigurator();

            //var TestCompList = MainPartview.RootComponents.SelectMany(x => x);
            //configurator.SetSource(MainPartview.RootComponents.SelectMany(x => x));

            //var a = 4; 
            #endregion


            #region Свойства поиска

            ////Создать класс поиска свойств
            //PropUI = new PropertyUI();
            ////Привязка к WPF
            //PropertyTab.DataContext = PropUI;
            ////Тестовый список хранения
            //PropUI.ComponentList = from comp in MainPartview.RootComponents
            //                       where comp.IsSelected
            //                       select comp.Appmodel;
            //Реализовать загрузку выделенных компонентов
            //PropUI.ComponentList = SelectedComp;
            #endregion
        }









        /// <summary>
        /// Выделить все компоненты в списке деталей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAll_click(object sender, RoutedEventArgs e)
        {
            MainModel.MainModelControl.CheckAllComponents();

        }

    }

    

}
