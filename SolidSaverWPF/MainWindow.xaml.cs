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
        public swPart sw_part { get; set; }
        public List<swPart> SwPartList { get; set; }
        public List<PropertyTemplate> PropList2 { get; set; }
        public PropertyTemplate PropObj { get; set; }

        public MainModel MainModel { get; set; }
        public IList<ISwProperty> PropList { get => MainModel.PropList; }
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

            SwPartList = TestClass.CreateTestPartList();
            //PartsList.ItemsSource = SwPartList;
            MainModel.GetSubComponents(false);
            PartsList.ItemsSource = SubComponents;



        }


        private void PartsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = PartsList.SelectedIndex;
            var currentPropList = SubComponents[index].PropList;
            //foreach (ISwProperty prop in currentPropList)
            //    prop.Update();
            PropertyBox.ItemsSource = currentPropList;
        }

    }

    public class swPart : ISwPartData
    {
        public List<PropertyTemplate> PropList { get; set; }
        public bool IsSelected { get; set; }
        public string PartType { get; set; }
        public string FileName { get; set; }

        public string TestProperty { get; set; }
        public string TestProperty2 { get; set; }

        
    }

    public static class TestClass
    {
        public static List<swPart> CreateTestPartList()
        {
            var partsList = new List<swPart>();
            partsList.Add(new swPart()
            {

                FileName = "TestFileName.sldprt",
                IsSelected = true, PartType = "Assembly",
                TestProperty = "Сборка 12345",
                PropList = new List<PropertyTemplate>()
                {
                    new PropertyTemplate
                    {
                        PrpName = "TestPropertyName",
                        PrpValue = "TestPropertyValue"
                    }
                }
            });

            partsList.Add(new swPart()
            {
                FileName = "TestFileName2.sldprt",
                IsSelected = false, PartType = "part",
                TestProperty = "Деталь 12345",
                TestProperty2 = "Второе свойство",
                PropList = new List<PropertyTemplate>()
                {
                    new PropertyTemplate
                    {
                        PrpName = "TestAaaaaName2",
                        PrpValue = "TestPropertyValue2"
                    }
                }
            });

            return partsList;
        }
    }

    public class PropertyTemplate
    {
        public string PrpName { get; set; }
        public string PrpValue { get; set; }
    }
}
