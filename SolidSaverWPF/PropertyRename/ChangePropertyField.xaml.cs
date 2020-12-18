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

namespace SolidSaverWPF.PropertyRename
{
    /// <summary>
    /// Interaction logic for ChangePropertyField.xaml
    /// </summary>
    public partial class ChangePropertyField : UserControl
    {
        public string OldValue { get; set; }
        private bool _valueWasChanged { get; set; }
        public ChangePropertyField()
        {
            InitializeComponent();

            PropCurrentValue.GotFocus += PropCurrentValue_GotFocus;
            //ConfigName
            //PropertyValue
            //ConfigName
            //IsModifyed
        }

        private void PropCurrentValue_GotFocus(object sender, RoutedEventArgs e)
        {
            if(_valueWasChanged == false)
            {
                OldValue = PropCurrentValue.Text;
                PropOldvalue.Text = OldValue;
                _valueWasChanged = true;
            }
        }
    }
}
