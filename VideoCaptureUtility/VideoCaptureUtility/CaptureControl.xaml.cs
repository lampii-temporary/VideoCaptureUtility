using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OculusFPV.Launcher
{
    /// <summary>
    /// Interaction logic for CaptureControl.xaml
    /// </summary>
    public partial class CaptureControl : Page
    {
        CaptureControlVM vm;
        public CaptureControl()
        {
            InitializeComponent();
             vm = new CaptureControlVM();
            this.DataContext = vm;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            vm.StartCapture();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            vm.StopCapture();
        }
        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm.RefreshList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vm.LatencyTest();
        }

    }
}
