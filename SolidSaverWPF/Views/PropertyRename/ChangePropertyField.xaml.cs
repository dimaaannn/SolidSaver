using System;
using System.Collections.Generic;
using System.Globalization;
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
        //public string OldValue { get; set; }
        //private bool _valueWasChanged { get; set; }
        public ChangePropertyField()
        {
            InitializeComponent();

        }

    }

    public class BoolToColorConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            if (status)
            {
                return new SolidColorBrush(Colors.LightYellow);
            }
            else return new SolidColorBrush(Colors.White);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
