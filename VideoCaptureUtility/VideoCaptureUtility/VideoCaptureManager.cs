using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OculusFPV.Launcher
{
    /// <summary>
    /// In charge of interacting with VMWrapper. It is a singleton for safety as this will be marshalling the image data
    /// from C/C++ to C#.
    /// 
    /// T
    /// </summary>
    /// 

    public sealed class VideoCaptureManager : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
            {
          //      Console.WriteLine("Updated Property " + e);
                PropertyChanged(this, new PropertyChangedEventArgs(e));
            }
        }
        #endregion

        private static readonly VideoCaptureManager instance = new VideoCaptureManager();
    
        private byte[] buffer1, buffer2;
        int defaultBufferSize = 1080 * 1920 * 3; //HD res just incase..
        Thread captureThread;
        volatile bool isCaptureRunning = false;
        List<Func<byte[], int>> _callbackMethods = new List<Func<byte[], int>>();

        int imageWidth = 0;
        int imageHeight = 0;

        public void AddCallback(Func<byte[], int> captureUpdate)
        {
            _callbackMethods.Add(captureUpdate);
        }

        public void RemoveCallback(Func<byte[], int> captureUpdate)
        {
            _callbackMethods.Remove(captureUpdate);
        }
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit

        #region Properties
        private int frameRate = 0;
        public int FrameRate { get { return frameRate; } set { frameRate = value; OnPropertyChanged("FrameRate"); } }

        private List<String> formatTypeList = Enum.GetNames(typeof(VIWrapper.FormatType)).ToList();

        private List<String> mediaTypeList = Enum.GetNames(typeof(VIWrapper.MSubType)).ToList();

        private List<String> physicalTypeList = Enum.GetNames(typeof(VIWrapper.PhysicalType)).ToList();
        public List<String> FormatTypeList { get { return formatTypeList; } }
        public List<String> PhysicalTypeList { get { return physicalTypeList; } }
        public List<String> MediaTypeList { get { return mediaTypeList; } }


        private VIWrapper.PhysicalType physicalType;
        public VIWrapper.PhysicalType RequestedPhysicalType
        {
            get { return physicalType; }
            set { physicalType = value; OnPropertyChanged("RequestedPhysicalType"); }
        }
        private VIWrapper.MSubType mediaSubtype;

        public VIWrapper.MSubType RequestedMediaType
        {
            get { return mediaSubtype; }
            set { mediaSubtype = value; VIWrapper.SetRequestedMediaSubType(mediaSubtype); OnPropertyChanged("RequestedMediaType"); }
        }
        private VIWrapper.FormatType formatType;

        public VIWrapper.FormatType RequestedFormatType
        {
            get { return formatType; }
            set { formatType = value; VIWrapper.SetFormat(deviceId, formatType); OnPropertyChanged("FormatType"); }
        }
        private int deviceId = 0; 

        public int DeviceID
        {
            get { return deviceId; }
            set { deviceId = value; OnPropertyChanged("DeviceID"); }
        }

        private int numDevices = 0;
        public int NumDevices
        {
            get { return numDevices; }
            set { numDevices = value; OnPropertyChanged("NumDevices"); }
        }

        private List<string> deviceNames = new List<string>();
        public List<String> DeviceNames
        {
            get { return deviceNames; }
            set { deviceNames = value; OnPropertyChanged("DeviceNames"); }
        }


        public int ImageHeight { get { return imageHeight; } set { imageHeight = value; UpdateSize(); OnPropertyChanged("ImageHeight"); } }
        public int ImageWidth { get { return imageWidth; } set { imageWidth = value; UpdateSize(); OnPropertyChanged("ImageWidth"); } }
        #endregion
       void UpdateSize()
        {
            ImageSize = imageWidth.ToString() + "x" + imageHeight.ToString();
        }
        
        string imageSize;
        public String ImageSize
        {
            get
            {
                return imageSize;
            }
            set
            {
                imageSize = value;
                OnPropertyChanged("ImageSize");
            }
        }
        private ImageSource image;

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
        static VideoCaptureManager()
        {

        }
        public void UpdateImage(BitmapSource source)
        {
            CapturedImage = source;
        }
        private int updateImage(byte[] newImage)
        {
            
            if (newImage != null)
            {
               
                    Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Render,
                                new Action(() =>
                                { 
                                    try
                                     {
                                    PixelFormat format = PixelFormats.Bgr24;
                                    VideoCaptureManager.Instance.CapturedImage = BitmapSource.Create(imageWidth, imageHeight, 96d, 96d, format, null, newImage, imageWidth * 3);
                                  }
                                catch (Exception e)
                                { }
                                }));
                    return 0;
             
                
              
            }
   
            return 1;
        }
        private int idealFrameRate = 60;
        public void StartCapture()
        {
            //Setup specific Device
            try
            {
                //lets be cautious with VIW
                //Check to see if ID is valid first.
                int num = VIWrapper.ListDevices();
                if (deviceId < num)
                {
                    VIWrapper.SetRequestedMediaSubType(mediaSubtype);
                    VIWrapper.SetFormat(deviceId, formatType);
                    VIWrapper.SetupDevice1(deviceId, physicalType);
                    VIWrapper.SetIdealFramerate(deviceId, idealFrameRate);

                    //Start the capture thread
                    captureThread = new Thread(new ParameterizedThreadStart(_capThread));
                    captureThread.Name = "CaptureThread";
                    captureThread.IsBackground = true;
                    isCaptureRunning = true;
                    captureThread.Start(deviceId);
                }
                

            }
            catch(Exception e)
            {
                Console.WriteLine("Exception occured while trying to start video capture. \n" + e);
            }
            //Turn on capture thread
        }
        public void StopCapture()
        {
     
          
                try
                {
                    LatencyTesterVM.Instance.KillPreview();
                    isCaptureRunning = false;
                    while (captureThread.IsAlive) //let the thread go peacefully.
                    {
                        Thread.Sleep(5);
                    }
                    Console.WriteLine("Shutting Down");
                    VIWrapper.StopDevice(DeviceID);
                }
                catch (Exception)
                { }
          
        }
        private VideoCaptureManager()
        {
            buffer1 = new byte[defaultBufferSize];
            formatType = VIWrapper.FormatType.NTSC_M;
             mediaSubtype = VIWrapper.MSubType.UYVY;
             physicalType = VIWrapper.PhysicalType.COMPOSITE;
             this.AddCallback(updateImage);
             RefreshList();
        }
        List<String> deviceList = new List<String>();
        public List<String> DeviceList { get { return deviceList; } set { deviceList = value; OnPropertyChanged("DeviceList"); } }

        public void RefreshList()
        {
            DeviceList.Clear();
            numDevices = VIWrapper.ListDevices();
            for (int i = 0; i < numDevices; i++)
            {
                DeviceList.Add(VIWrapper.GetDeviceName(i));
            }
            OnPropertyChanged("DeviceID");
            OnPropertyChanged("DeviceList");
        }
        IntPtr unmanagedImagePtr;
        private readonly object bufferGuard = new object();

        private readonly object guard2 = new object();
        private void _capThread(object deviceID)
        {
            int devId = (int)deviceID;

            while(isCaptureRunning)
            {
               
                 
                        UpdateFrameRate();//update the framerate.
                        Thread.Sleep(1);
                        if (isCaptureRunning && VIWrapper.IsFrameReady(devId))
                        {
                            unmanagedImagePtr = IntPtr.Zero; // Clean the pointer!
                            try //Enter exception safetyland!
                            {
                                VIWrapper.GetImage(devId, ref unmanagedImagePtr, false, true);

                                //Update Width and Height - Probably not necessary after capture starts. Move to StartCapture!
                                ImageWidth = VIWrapper.GetWidth(devId);
                                ImageHeight = VIWrapper.GetHeight(devId);

                                int bytesPerPixel = 3;//NOTE:Assumption of RGB24 starts here!

                                lock (bufferGuard) /* Enter critical section to copy data into buffer */
                                {
                                    if (unmanagedImagePtr != IntPtr.Zero)
                                    {
                                        unsafe
                                        {
                                            Marshal.Copy(unmanagedImagePtr, buffer1, 0, (imageWidth * imageHeight * bytesPerPixel));
                                        }
                                    }
                                }

                                //      Marshal.FreeHGlobal(unmanagedImagePtr); //Free the data since it should be managed.
                                GC.Collect(); //Enforced GC because 60frames passes a TON of data

                                //Now, call back registered members!
                                //Maybe Lock each call?? If i get crashes, try that.
                                foreach (Func<byte[], int> function in _callbackMethods)
                                {
                                    Application.Current.Dispatcher.BeginInvoke(
                   DispatcherPriority.Loaded,
                   new Action(() =>
                   {
                       try
                       {
                           function(buffer1);
                       }
                       catch (Exception e)
                       { }
                   }));
                     
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("CAPTURE FAILURE please report to dev!. Exception -->" + e);
                                Console.WriteLine("Exception Source: " + e.Source);
                                Console.WriteLine("Stack Trace:" + e.StackTrace);
                            }
                        }
                    }
               
            }
        
        DateTime previousTime;
        int lastFPS = 0;
        int fpsCounter = 0;
        private void UpdateFrameRate()
        {

            FrameRate = lastFPS;
            TimeSpan duration = DateTime.Now - previousTime;
            if (duration.TotalMilliseconds >= 1000)
            {
                lastFPS = fpsCounter;
                fpsCounter = 0;
                previousTime = DateTime.Now;
            }

            fpsCounter++;
        }

        public static VideoCaptureManager Instance
        {
            get
            {
                return instance;
            }
        }


    }
}
