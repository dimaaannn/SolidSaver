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
using GalaSoft.MvvmLight.Messaging;
using SolidSaverWPF.MessagesType;
using SWAPIlib.Global;

namespace SolidSaverWPF.Views
{
    /// <summary>
    /// Interaction logic for PartListView.xaml
    /// </summary>
    public partial class PartListView : UserControl
    {

        public PartListView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Компонент выбран в списке
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void SelectedItemChanged(object o, RoutedPropertyChangedEventArgs<object> e)
        {
            Messenger.Default.Send<SelectionMessage<object>>(
                    new SelectionMessage<object>(this, e.NewValue));
        }

    }
}
