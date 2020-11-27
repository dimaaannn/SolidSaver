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
        public List<PropertyTemplate> PropList { get; set; }
        public PropertyTemplate PropObj { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SwPartList = TestClass.CreateTestPartList();
            PartsList.ItemsSource = SwPartList;

            sw_part = new swPart
            {
                FileName = "TestName",
                IsSelected = false,
                TestProperty = "TestProp"
            };

            var propTemplate = new PropertyTemplate
            {
                PrpName = "TestPropertyName",
                PrpValue = "TestPropertyValue"
            };

            PropList = new List<PropertyTemplate>() { propTemplate };

            this.DataContext = sw_part;
            this.DataContext = PropObj;

            PropertyListBox.ItemsSource = PropList;
        }


        private void PartsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = PartsList.SelectedIndex;
            PropertyListBox.ItemsSource = SwPartList[index].PropList;
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
