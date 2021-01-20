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

namespace SolidSaverWPF.PartList
{
    /// <summary>
    /// Interaction logic for PartListView.xaml
    /// </summary>
    public partial class PartListView : UserControl
    {
        SWAPIlib.Global.IMainPartView _mainPartView;
        public SWAPIlib.Global.IMainPartView MainPartView
        {
            get => _mainPartView;
            set => _mainPartView = value;
        }
        public PartListView()
        {
            InitializeComponent();
            //MainPartView.Rootmodel.Title
        }

        private void TreePartView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MainPartView.SelectedComp = ((SWAPIlib.Controller.IComponentControl)e.NewValue);
        }
    }
}
