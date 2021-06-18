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

namespace SolidSaverWPF.Views.Table
{
    /// <summary>
    /// Interaction logic for CellView.xaml
    /// </summary>
    public partial class CellView : UserControl
    {
        public CellView()
        {
            InitializeComponent();
        }


        private void PropCurrentValue_GotFocus(object sender, RoutedEventArgs e)
        {
            var brush = new SolidColorBrush(Colors.White) { Opacity = 0.7 };
            PropCurrentValue.Background = brush;
        }

        private void PropCurrentValue_LostFocus(object sender, RoutedEventArgs e)
        {
            PropCurrentValue.Background = Brushes.Transparent;
        }

    }

    public class NullToColorConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = String.IsNullOrEmpty((string)value);
            if (!status)
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

    /// <summary>
    /// После изменения текста сделать записанное значение серым
    /// </summary>
    public class ValueTextColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool notNull = !String.IsNullOrEmpty((string)value);
            if (notNull)
            {
                return new SolidColorBrush(Colors.LightGray);
            }
            else return new SolidColorBrush(Colors.Black);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
