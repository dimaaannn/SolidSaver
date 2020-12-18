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

namespace SolidSaverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainModel MainModel { get; set; }
        public IList<ISwProperty> PropList { get; set; }
        public ObservableCollection<IAppComponent> SubComponents { get => MainModel.SubComponents2; }
        public AppComponent SelectedModel { get; set; }

        public PropModifier TestProperty { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            MainModel = new MainModel();
            this.DataContext = MainModel;

            SwAppControl.Connect();
            MainModel.GetMainModel();

            //PartsList.ItemsSource = SwPartList;
            MainModel.GetSubComponents();
            PartsList.ItemsSource = SubComponents;

            MainModel.TopLevelOnly = true;

            //Component property tests

            var testcomponent = SubComponents[1];
            TestProperty = new PropModifier(
                testcomponent.PartModel,
                SWAPIlib.PropertyObj.PropFactory.Nomination
                );
            TestProperty.AllConfiguration = true;

            var TestPropPair = TestProperty.SwPropList.First();
            PropConfigName.Text = TestPropPair.Key;

            var bind = new Binding("Value.PropertyValue");
            //TestPropPair.Value.PropertyValue
            bind.Source = TestPropPair;

            PropConfigvalue.SetBinding(TextBox.TextProperty, bind);

            //testSwProp.PropertyValue
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
    }

    

}
