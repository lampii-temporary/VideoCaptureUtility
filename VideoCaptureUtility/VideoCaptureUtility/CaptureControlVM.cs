using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OculusFPV.Launcher
{

    class CaptureControlVM: INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(e));
            }
        }
        #endregion

        #region Members
        static byte[] array = new byte[1080 * 1920 * 3];
        private int lastFrameRate = 0;
        private int frameRate = 0;
        Thread capThread;
        private ImageSource image;
        bool runTest = false;
        private int numDevices = 0;
        private int lastTick = 0;
        bool keepGoing = true;
        IntPtr output;
        int redThreshold = 110;
        int whiteThresh = 50;

        DateTime last;
        DateTime now;
        Stopwatch redDetected = new Stopwatch();
        // Do stuff
        int counter = 0;
        int prev = 0;
        #endregion


        #region Properties
        public ImageSource Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                OnPropertyChanged("Image");
            }
        }
        public ImageSource CapturedImage
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                OnPropertyChanged("CapturedImage");
            }
        }
        public int FrameRate
        {
            get
            {
                return lastFrameRate;
            }
            set
            {
                lastFrameRate = value;
                OnPropertyChanged("FrameRate");
            }
        }
        public int DeviceCount
        {
            get
            { return numDevices; }
            set { numDevices = value; OnPropertyChanged("DeviceCount"); }
        }
        public List<String> DeviceList { get; set; }
        public List<String> MediaTypes { get; set; }
        public List<String> PhysTypes { get; set; }
        public List<String> FormatTypes { get; set; }
        #endregion

        public CaptureControlVM()
        {
           
           FormatTypes = Enum.GetNames(typeof(FormatType)).ToList();
           PhysTypes = Enum.GetNames(typeof(PhysicalType)).ToList();
           MediaTypes = Enum.GetNames(typeof(MSubType)).ToList();
           DeviceList = new List<string>();
           RefreshList();
           frameRate = 0;

        }
      

        public void GenerateConnectionParams()
        {
  
        }

        private void _captureThread(object devIdf)
        {
            int devId = (int)devIdf;
            while (keepGoing)
            {
                FrameRate = CalculateFrameRate();

                Thread.Sleep(1);
                if (VIWrapper.IsFrameReady(selectedDeviceIndex))
                {
                //   
                //    writeLock = true;
                
                    output = IntPtr.Zero;
                            VIWrapper.GetImage(devId, ref output, false, true);
                            writeLock = false;
                            int width = VIWrapper.GetWidth(devId);
                            int height = VIWrapper.GetHeight(devId);
                        try
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                          DispatcherPriority.Background,
                          new Action(() =>
                          {
                              CapturedImage = FromNativePointer(output, width, height, 3);
                             
                          }));
                            if (this.runTest)
                            {
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                if (CheckIfRed(this.redThreshold, array, width, height, 3))
                                {
                                    redDetected.Stop();
                                    EndLatencyTest();

                                }
                                sw.Stop();
                      //          Console.WriteLine("It took " + sw.ElapsedTicks + " to run or" + sw.ElapsedMilliseconds + " Ms");
                            }
                        }
                    catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }
                 }
            }
        }
        void EndLatencyTest()
        {
            runTest = false;
            Console.WriteLine("Elapsed time: {0}ms {1}ticks", redDetected.ElapsedMilliseconds, redDetected.ElapsedTicks);

        }
 
        public  int CalculateFrameRate()
        {

            FrameRate = prev;
            TimeSpan duration = DateTime.Now - last;
            if(duration.TotalMilliseconds >= 1000)
            {
                prev = counter;
                counter = 0;
                last = DateTime.Now;
            }
         
            counter++;
            return lastFrameRate;
        }

        /// <summary>
        /// Starts capturing the selected device.
        /// </summary>
        public void StartCapture()
        {
            Console.WriteLine("Starting Capture");
            if(capThread != null)
            {
                capThread.Abort();
            }
            VIWrapper.SetupDevice(selectedDeviceIndex);
            keepGoing = true;
            capThread = new Thread(new ParameterizedThreadStart(_captureThread));
            capThread.Start(selectedDeviceIndex);
          
        }

        public bool CheckIfRed(int thresh, byte[] array, int width, int height, int stride)
        {
            //Assuming RGB(3)
            int size = width * height;
            int startXLoc = 2;
            int xLength = size/3; //Only do like a third of the image..
            int count = 0;
            int sum = 0;
            int avg = 0;
            for (int x = startXLoc; x < xLength; x += stride)
            {
                count++;
                sum += array[x];
                avg = sum / count;
            }
            Console.WriteLine("Counted " + count + " red values with sum of" + sum + " and avg of " + avg);

            if (thresh < avg)
            {
                return true;
            }
                return false;
    
        }
        public bool CheckIfWhite(int thresh, byte[] array, int width, int height, int stride)
        {
            //Assuming RGB(3)
            int size = width * height;
            int startXLoc = 2;
            int xLength = size / 3; //Only do like a third of the image..
            int count = 0;
            int sum = 0;
            int avg = 0;
            for (int x = startXLoc; x < xLength; x++)
            {
                count++;
                sum += array[x];
                avg = sum / count;
            }
            Console.WriteLine("Counted " + count + " values with sum of" + sum + " and avg of " + avg);

            if (thresh < avg)
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// Stops capturing the selected device
        /// </summary>
        public void StopCapture()
        {
            Console.WriteLine("Stopping Device.");
            keepGoing = false;

        }


        private string selectedPhysType = "AUTO";
        private string selectedMediaType = "AUTO";
        public String SelectedPhysType
        {
            get { return selectedPhysType; }
            set
            {
                selectedPhysType = value;
                OnPropertyChanged("SelectedPhysType");
            }
        }
        public String SelectedMediaType
        {
            get { return selectedMediaType; }
            set
            {
                selectedMediaType = value;
                OnPropertyChanged("SelectedMediaType");
            }
        }
        private int selectedDeviceIndex = 0;
        public int SelectedDeviceIndex
        {
            get { return selectedDeviceIndex; }
            set
            {
                selectedDeviceIndex = value;
                Console.WriteLine("Selected Index is" + selectedDeviceIndex);
                OnPropertyChanged("SelectedDeviceIndex");
            }
        }
        private string selectedFormat = "AUTO";
        public String SelectedFormat { get { return selectedFormat; } 
            set 
            { 
                selectedFormat = value;
                OnPropertyChanged("SelectedFormat");
            }
        }
        public void RefreshList()
        {
            DeviceList.Clear();
            numDevices = VIWrapper.ListDevices();
            for (int i = 0; i < numDevices; i++)
            {
                DeviceList.Add(VIWrapper.GetDeviceName(i));
            }
        }

        void _latencySamplerThread()
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //int strid = ((BitmapSource)image).PixelWidth *4;
            //int size = strid*(int)image.Height;
            //int offset =(int)( size * 4);
          
            //((BitmapSource) image).CopyPixels(array,strid,0);

           


        }
        public void LatencyTest()
        {
            LatencyTesterWindow ltWin = new LatencyTesterWindow();
            LatencyTestView testView = new LatencyTestView();
            //testView.Show();
            //ltWin.ChangeBrush(LatencyTesterWindow.BColor.White);
            //Thread.Sleep(2000);
            //ltWin.ChangeBrush(LatencyTesterWindow.BColor.Red);

            //redDetected = new Stopwatch();
            //redDetected.Start();
            //runTest = true;
        }


        public enum MSubType : int
        {
            AUTO = 99,
            RGB24 = 0,
            RGB32 = 1,
            RGB555 = 2,
            RGB565 = 3,
            YUY2 = 4,
            YVYU = 5,
            YUYV = 6,
            IYUV = 7,
            UYVY = 8,
            YV12 = 9,
            YVU9 = 10,
            Y411 = 11,
            Y41P = 12,
            Y211 = 13,
            AYUV = 14,
            Y800 = 15,
            Y8 = 16,
            GREY = 17,
            MJPG = 18
        };
        enum PhysicalType: int
        {
            AUTO = 99,
            COMPOSITE = 0,
            S_VIDEO = 1,
            TV_TUNER= 2,
            USB = 3,
            FW1394 = 4
        }
        enum FormatType : int
        {
            AUTO = 99,
            NTSC_M=	0,
            PAL_B	=1,
            PAL_D	=2,
            PAL_G	=3,
            PAL_H	=4,
            PAL_I	=5,
            PAL_M	=6,
            PAL_N	=7,
            PAL_NC	=8,
            SECAM_B	=9,
            SECAM_D	=10,
            SECAM_G	=11,
            SECAM_H	=12,
            SECAM_K	=13,
            SECAM_K1	=14,
            SECAM_L	=15,
            NTSC_M_J	=16,
            NTSC_433	=17,
        };
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);
        static bool writeLock = false;
        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern int GdipCreateBitmapFromHBITMAP(HandleRef hbitmap,
          HandleRef hpalette, out IntPtr bitmap);

        public static BitmapSource FromNativePointer(IntPtr pData, int w, int h, int ch)
        {

            writeLock = true;
            GC.Collect();
                PixelFormat format = PixelFormats.Default;

                if (ch == 1) format = PixelFormats.Gray8; //grey scale image 0-255
                if (ch == 3) format = PixelFormats.Bgr24; //RGB
                if (ch == 4) format = PixelFormats.Bgr32; //RGB + alpha

                if (pData != IntPtr.Zero)
                {
             
                Marshal.Copy(pData, array, 0, (w * h * ch));
                }
                var width = w; // for example
                var height = h; // for example
                var dpiX = 96d;
                var dpiY = 96d;
                var bytesPerPixel = (format.BitsPerPixel + 7) / 8;
                var stride = bytesPerPixel * width;

                var bitmap = BitmapSource.Create(width, height, dpiX, dpiY,
                                                 format, null, array, stride);

                

                writeLock = false;
            
            return bitmap;
        }
    }
    public class MSubTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (OculusFPV.Launcher.CaptureControlVM.MSubType)Enum.Parse(typeof(OculusFPV.Launcher.CaptureControlVM.MSubType), value.ToString(), true);
        }
    }

}
