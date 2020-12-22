using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using SWAPIlib.PropertyObj;

namespace SolidSaverWPF.PropertyRename
{
    /// <summary>
    /// Interaction logic for PropertyChangerTab.xaml
    /// </summary>
    public partial class PropertyChangerTab : UserControl
    {
        public PropertyChanger PropChanger
        {
            get
            {
                if (_propChanger == null)
                    _propChanger = (PropertyChanger)this.DataContext;

                return _propChanger;
            }
        }
        protected PropertyChanger _propChanger;
        public PropertyChangerTab()
        {
            InitializeComponent();


            SearchValues.Click += SearchValues_Click;
            WriteValues.Click += WriteValues_Click;
            RestoreValues.Click += RenewValues_Click;
        }

        private void RenewValues_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Renew values button click");
            PropChanger.RestoreValues();
        }

        private void WriteValues_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Write button click");
            PropChanger.WriteValues();
        }

        private void SearchValues_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Search button click");
            PropChanger.ProceedValues();
        }
    }
}
