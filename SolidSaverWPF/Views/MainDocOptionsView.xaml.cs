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

namespace SolidSaverWPF.Views
{
    /// <summary>
    /// Interaction logic for MainDocOptionsView.xaml
    /// </summary>
    public partial class MainDocOptionsView : UserControl
    {
        public MainDocOptionsView()
        {
            InitializeComponent();
        }

        private void WorkFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            //WorkFolderField.CaretIndex = WorkFolderField.Text.Length;
            //WorkFolderField.ScrollToEnd();
            WorkFolderField.ScrollToHorizontalOffset(1000);
            
        }

    }
}
