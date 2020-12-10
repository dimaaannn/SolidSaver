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

namespace SolidSaverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainModel MainModel { get; set; }
        public IList<ISwProperty> PropList { get; set; }
        public List<IAppComponent> SubComponents { get => MainModel.SubComponents; }
        public AppComponent SelectedModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainModel = new MainModel();
            this.DataContext = SelectedModel;
            this.DataContext = PropList;
            this.DataContext = SubComponents;
            this.DataContext = MainModel;

            SwAppControl.Connect();
            MainModel.GetMainModel();

            //PartsList.ItemsSource = SwPartList;
            MainModel.GetSubComponents(false);
            PartsList.ItemsSource = SubComponents;



        }


        private void PartsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = PartsList.SelectedIndex;
            var currentPropList = SubComponents[index].PropList;
            PropList = currentPropList;
            PropertyBox.ItemsSource = currentPropList;
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            foreach(var prop in PropList)
            {
                prop.Update();
                
            }

        }

        private void ButtonWrite_Click(object sender, RoutedEventArgs e)
        {
            foreach (var prop in PropList)
                prop.WriteValue();
        }

        private void ButtonWriteAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var comp in SubComponents)
            {
                foreach (var prop in comp.PropList)
                    prop.WriteValue();
            }
        }
    }



}
