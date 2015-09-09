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
using System.Windows.Shapes;

namespace OculusFPV.Launcher
{
    /// <summary>
    /// Interaction logic for LatencyTestView.xaml
    /// </summary>
    public partial class LatencyTestView : UserControl
    {
        LatencyTesterVM vm = LatencyTesterVM.Instance;
       
        //TODO Views should be independent period. Maybe the wrapper should provide a good interface?
        public LatencyTestView()
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void compOperator_Unchecked(object sender, RoutedEventArgs e)
        {
            compOperator.Content = "less";
        }

        private void compOperator_Checked(object sender, RoutedEventArgs e)
        {
            compOperator.Content = "greater";

        }
        //preview
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vm.PreviewPressed();
        }

    
    }
}
