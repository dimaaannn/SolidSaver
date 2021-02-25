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
        /// Управление коренной моделью сборки
        /// </summary>
        public IRootModel MainModel { get; set; }
        /// <summary>
        /// Список деталей основной модели
        /// </summary>
        public SWAPIlib.Global.IMainPartViewControl MainPartview { get; set; }
        /// <summary>
        /// Поиск и замена свойств
        /// </summary>
        public IPropertyUI PropUI { get; set; }

        ///В доработку
        public List<IAppComponent> SubComponents { get => MainModel.SubComponents; }
        public AppComponent SelectedModel { get; set; }
        public List<IPropertyModel> PropList { get; set; }

        //Tests
        public KeyValuePair<string, ISwProperty> TestPropPair { get; set; }


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
            MainModel = new RootModel();
            this.DataContext = this;

            SwAppControl.Connect();
            MainModel.GetMainModel();
            MainModel.TopLevelOnly = true;
            MainModel.GetSubComponents();

            //Основной список деталей
            //Создать класс
            MainPartview = new SWAPIlib.Global.MainPartViewControl(MainModel);
            ///Подключить к списку деталей WPF - Заменить на binding
            //PartViewList.MainPartView = MainPartview;
            //Fix binding bug
            //Bind Main Part list to interface
            PartViewList.DataContext = MainPartview;
            PropertyBox.DataContext = MainPartview;
            //MainPartview.RootComponents
            //PropertyBox.ItemsSource = MainPartview.SelectedCompProp;

            var configurator = PropertyView.PropConfigurator.GetConfigurator();

            var TestCompList = MainPartview.RootComponents.SelectMany(x => x);
            configurator.SetSource(MainPartview.RootComponents.SelectMany(x => x));

            var a = 4;
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
        /// Обновить свойства активной детали
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            PropertyBox.ItemsSource = MainPartview.SelectedCompProp;
            //foreach(var prop in PropList)
            //{
            //    prop.Update();
            //}
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

        /// <summary>
        /// Кнопка "Загрузить модель"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            MainModel.LoadActiveModel();
            MainModel.GetSubComponents();
        }

        /// <summary>
        /// Выделить все компоненты в списке деталей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAll_click(object sender, RoutedEventArgs e)
        {
            foreach (var comp in MainPartview)
            {
                comp.IsSelected = !comp.IsSelected;
            }

        }

        /// <summary>
        /// Очистить список деталей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach(var comp in MainPartview.RootComponents)
            {
                comp.SubComponents.Clear();
            }
        }

        /// <summary>
        /// Перезагрузить список деталей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainPartview.ReloadCompList();
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
