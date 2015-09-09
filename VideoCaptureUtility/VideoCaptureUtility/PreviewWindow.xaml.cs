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
using System.Windows.Threading;

namespace OculusFPV.Launcher
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public PreviewWindow()
        {
            InitializeComponent();
        }

        public void UpdateDetails(string iD, string name)
        {
            nameLabel.Content = name;
            idValLabel.Content= iD;
        }
        public int UpdateImage(byte[] newImage)
        {
            if (newImage != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        try
                        {
                            PixelFormat format = PixelFormats.Bgr24;
                            image.Source = BitmapSource.Create(VideoCaptureManager.Instance.ImageWidth, VideoCaptureManager.Instance.ImageHeight, 96d, 96d, format, null, newImage, VideoCaptureManager.Instance.ImageWidth * 3);
                        }
                        catch (Exception) { }
                    }));
                return 0;
            }
            else
                return 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VideoCaptureManager.Instance.RemoveCallback(UpdateImage);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDetails(VideoCaptureManager.Instance.DeviceID.ToString(), VideoCaptureManager.Instance.DeviceList[VideoCaptureManager.Instance.DeviceID]);
            VideoCaptureManager.Instance.AddCallback(UpdateImage);

        }
    }
}
