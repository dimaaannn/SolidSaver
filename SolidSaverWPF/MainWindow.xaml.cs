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
        public static bool IsInDebugMode { get; private set; }
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SwAppControl.Connect();

//#if DEBUG
            IsInDebugMode = true;
//#endif

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

        private void DebugButtonClick(object sender, RoutedEventArgs e)
        {
            var window = new Tests.DebugWindow();
            window.Show();
        }
    }

    

}
