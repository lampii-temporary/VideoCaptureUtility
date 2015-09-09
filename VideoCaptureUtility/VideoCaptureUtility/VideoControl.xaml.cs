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

namespace OculusFPV.Launcher
{
    /// <summary>
    /// Interaction logic for VideoControl.xaml
    /// </summary>
    public partial class VideoControl : UserControl
    {
        VideoCaptureManager captureManager;
        public VideoControl()
        {
            InitializeComponent();
            captureManager = VideoCaptureManager.Instance;
            this.DataContext = captureManager;
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            captureManager.RefreshList();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void startButton_Checked(object sender, RoutedEventArgs e)
        {
            captureManager.StartCapture();
            startButton.Content = "Stop";
            deviceListCombo.IsEnabled = false;

        }

        private void startButton_Unchecked(object sender, RoutedEventArgs e)
        {
            captureManager.StopCapture();
            startButton.Content = "Start";
            deviceListCombo.IsEnabled = true;

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
