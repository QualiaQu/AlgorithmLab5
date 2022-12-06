using System;
using System.Windows;

namespace Graph.WPF
{
    public partial class WeightInput : Window
    {
        public WeightInput()
        {
            InitializeComponent();
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
 
        public int Weight => Convert.ToInt32(WeightBox.Text);
    }
}