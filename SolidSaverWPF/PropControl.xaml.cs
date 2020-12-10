using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SolidSaverWPF.Prop
{
    /// <summary>
    /// Interaction logic for PropControl.xaml
    /// </summary>
    public partial class PropControl : UserControl
    {
        public Binding PropValueBinding;

        public PropControl()
        {
            InitializeComponent();

            //PropValueBinding = BindingOperations.GetBinding(PropValue, TextBox.TextProperty);

            UpdateBtn.Click += UpdateBtn_Click;
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Save property button click");
            var prop = ((SWAPIlib.ISwProperty)this.DataContext);
            prop.WriteValue();
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            if (status)
            {
                return new SolidColorBrush(Colors.OrangeRed);
            }
            else return new SolidColorBrush(Colors.White);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
