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
        public MainWindow()
        {
            InitializeComponent();
            PartsList.ItemsSource = TestClass.CreateTestPartList();
        }
    }

    public class swPart : ISwPartData
    {
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
                TestProperty = "Сборка 12345"
            });

            partsList.Add(new swPart()
            {
                FileName = "TestFileName2.sldprt",
                IsSelected = false, PartType = "part",
                TestProperty = "Деталь 12345",
                TestProperty2 = "Второе свойство"
            });

            return partsList;
        }
    }
}
