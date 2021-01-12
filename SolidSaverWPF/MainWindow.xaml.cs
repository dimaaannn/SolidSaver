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
using SWAPIlib.VisualInterface;
using SWAPIlib.Controller;


namespace SolidSaverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public IMainModel MainModel { get; set; }
        public IList<ISwProperty> PropList { get; set; }
        public List<IAppComponent> SubComponents { get => MainModel.SubComponents; }
        public AppComponent SelectedModel { get; set; }
        public IPropertyUI PropUI { get; set; }

        //Tests
        /// <summary>
        /// Выбранные компоненты для загрузки в список свойств
        /// </summary>
        public List<IAppComponent> SelectedComp { get; } = new List<IAppComponent>();
        public KeyValuePair<string, ISwProperty> TestPropPair { get; set; }
        public ThreePartList ThreePart { get; set; }
        public ObservableCollection<ComponentControl> CompControlList;

        public PartList<IAppComponent> 
            TestPartList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainModel = new MainModel();
            //this.DataContext = MainModel;
            this.DataContext = this;

            SwAppControl.Connect();
            MainModel.GetMainModel();
            MainModel.TopLevelOnly = true;
            MainModel.GetSubComponents();

            PropUI = new PropertyUI();
            PropertyTab.DataContext = PropUI;
            PartsList.ItemsSource = SubComponents;
            PropUI.ComponentList = SelectedComp;

            CompControlList = new ObservableCollection<ComponentControl>();

            ThreePart = new ThreePartList();
            foreach (var comp in SubComponents)
            {
                CompControlList.Add(new ComponentControl(comp));
            }
            TestPartView.TreePartView.ItemsSource = CompControlList;
            //TestThreeview.ItemsSource = CompControlList;



            //CompControlList[0].PartType
            //threPL.DataContext = ThreePart;

            //Tests
            //PropUI.ConstructorDict.Keys
            //PartsList.ItemsSource = SwPartList;

            TestPartList = new PartList<IAppComponent>();
            foreach (var appcomp in SubComponents)
            {
                var temp = new ComponentControl(appcomp);
                TestPartList.PartCollection.Add(temp);
                //TestPartList.SelectionNum
            }

            

            
            //Run new thread
            //System.Threading.ThreadPool.QueueUserWorkItem(TestPartList.ChangeSelection);
        }


        private void PartsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = PartsList.SelectedIndex;
            if(index >= 0)
            {
                var currentPropList = SubComponents[index].PropList;
                PropList = currentPropList;
                PropertyBox.ItemsSource = currentPropList;
            }
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

        private void PartsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var index = PartsList.SelectedIndex;
            if (index >= 0)
            {
                var currentComp = SubComponents[index];
                SelectedComp.Add(currentComp);
            }
        }
    }

    

}
