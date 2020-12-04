﻿using System;
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

namespace SolidSaverWPF.Prop
{
    /// <summary>
    /// Interaction logic for PropControl.xaml
    /// </summary>
    public partial class PropControl : UserControl
    {
        Binding ColorBinding { get; set; }
        Brush YellowBrush;

        public PropControl()
        {
            InitializeComponent();
            //PropValue.TextChanged += PropValue_TextChanged;
            //BindingColor();

        }

        private void BindingColor()
        {
            ColorBinding = new Binding("_tempPropertyValue");
        }

        //private void PropValue_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if(ColorBinding.StringFormat != PropValue.Text)
        //        PropValue.Background 
        //}
    }
}
