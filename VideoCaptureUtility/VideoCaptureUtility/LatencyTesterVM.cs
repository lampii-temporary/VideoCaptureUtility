using OculusFPV.Launcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
namespace OculusFPV.Launcher
{
    public class UniversalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // obtain the conveter for the target type
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                // determine if the supplied value is of a suitable type
                if (converter.CanConvertFrom(value.GetType()))
                {
                    // return the converted value
                    return converter.ConvertFrom(value);
                }
                else
                {
                    // try to convert from the string representation
                    return converter.ConvertFrom(value.ToString());
                }
            }
            catch (Exception)
            {
                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class LatencyTesterVM: INotifyPropertyChanged
    {
        private static readonly LatencyTesterVM instance = new LatencyTesterVM();
        public static LatencyTesterVM Instance
        {
            get
            {
                return instance;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
            {
               // Console.WriteLine(e + " changed");
                PropertyChanged(this, new PropertyChangedEventArgs(e));
            }
        }

        /// <summary>
        /// Because Wpf doesn't want structs..
        /// </summary>
        public class RGBVal : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string e)
            {
                if (PropertyChanged != null)
                {
                    Console.WriteLine(e + " changed");
                    PropertyChanged(this, new PropertyChangedEventArgs(e));
                }
            }
        
            int r=0, b=0, g=0;
            public RGBVal()
            { }
            public RGBVal(int R,int B, int G)
            {
                r = R; 
                b = B; 
                g = G;
            }
            public int R { get{ return r;} set{ r = value; OnPropertyChanged("R"); }}
            public int B { get{ return b;} set{ b = value; OnPropertyChanged("B"); }}
            public int G { get{ return g;} set{ g = value; OnPropertyChanged("G"); }}

        }

        #region Members

        bool alwaysSample = true;
        public bool AlwaysSample
        {
            get
            {
                return alwaysSample;
            }
            set
            {
                alwaysSample = value;
                OnPropertyChanged("AlwaysSample");
            }
        }
        PreviewWindow previewWindow;

        public void PreviewPressed()
        {

            if (previewWindow != null)
            {
                try
                {
                    previewWindow.Close();
                    previewWindow = null; 
                }
                catch (Exception) { }
            }
           
                previewWindow = new PreviewWindow();
                previewWindow.Show();


        }
        public void KillPreview()
        {
            if(previewWindow != null)
            {
                try
                {
                    previewWindow.Close();
                    previewWindow = null;
                }
                catch (Exception) { }
            }
        }
        RGBVal colorThreshold;
        RGBVal colorAverage;
        Stopwatch stopwatch;

        #endregion
        public LatencyTesterVM()
        {
            //captureControl = control;
            stopwatch = new Stopwatch();
            rectColor = Brushes.DarkGray;
            colorThreshold = new RGBVal();
            colorThreshold.R = 150;
            colorThreshold.G = 0;
            colorThreshold.B = 0;
            colorAverage = new RGBVal();
            colorAverage.R = 0;
            colorAverage.G = 0;
            colorAverage.B = 0;
            BaseColor = Brushes.Black;
            TestColor = Brushes.Red;
       //     VideoCaptureManager.Instance.AddCallback(sampleGreaterThan);
        }
        public RGBVal ColorThresh
        {
            get
            {
                return colorThreshold;
            }
            set
            {
                colorThreshold = value;
                OnPropertyChanged("ColorThresh");

            }
        }
        Brush baseColor, testColor;
        public System.Windows.Media.Brush TestColor
        {
            get { return testColor; }
            set { testColor = value; OnPropertyChanged("TaseColor"); }
        }
        public System.Windows.Media.Brush BaseColor
        {
            get { return baseColor; }
            set { baseColor = value; OnPropertyChanged("BaseColor"); }
        }
        public RGBVal ColorAverage
        {
            get
            {
                return colorAverage;
            }
            set
            {
                colorAverage = value;
                OnPropertyChanged("ColorAverage");

            }
        }
        Brush rectColor;
        Brush RectColor
        {
            get
            {
                return rectColor;
            }
            set
            {
                rectColor = value;
                OnPropertyChanged("RectColor");

            }
        }
        #region Properties
        List<String> m_colorList;
        List<String> Colors
        {
            get { return m_colorList; }
            set
            {
                m_colorList = value; OnPropertyChanged("Colors");
            }
        }
        #endregion

        public void RunTest()
        {

        }
        public int sampleLessThan(byte[] array)
        {
            RGBVal sample = ColorSampler.Sample3Channels(array, VideoCaptureManager.Instance.ImageWidth, VideoCaptureManager.Instance.ImageHeight);
            this.ColorAverage = sample;
            if(sample.R < ColorThresh.R && sample.G < ColorThresh.G && sample.B < ColorThresh.B)
            {
                return 1;
            }

            return 0;
        }
        public int sampleGreaterThan(byte[] array)
        {
            RGBVal sample = ColorSampler.Sample3Channels(array, VideoCaptureManager.Instance.ImageWidth, VideoCaptureManager.Instance.ImageHeight);
            this.ColorAverage = sample;
            if (sample.R > ColorThresh.R && sample.G > ColorThresh.G && sample.B > ColorThresh.B)
            {
                VideoCaptureManager.Instance.RemoveCallback(sampleGreaterThan);
                return 1;
            }

            return 0;
        }
  
    
    }
    public class ColorSampler
    {
        /// <summary>
        /// Samples the avg RGB values of the entire image
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static LatencyTesterVM.RGBVal Sample3Channels(BitmapSource source)
        {
            PixelFormat format = source.Format;
            LatencyTesterVM.RGBVal result = default(LatencyTesterVM.RGBVal);// = new LatencyTesterVM.RGBVal();
            LatencyTesterVM.RGBVal sum = default(LatencyTesterVM.RGBVal);// = new LatencyTesterVM.RGBVal();

            int count= 0;

            int width = (int)source.Width ;
            int height = (int)source.Height ;
            int size = width * height;
            byte[] pixels = new byte[size];
            for (int i = 0; i < size; i+=2 )
            {
                sum.R += pixels[i];
                sum.G += pixels[i];
                sum.B += pixels[i];
                count++;
            }
                //TODO Stop assuming RGB format
            result.R = sum.R / count;
            result.G = sum.G / count;
            result.B = sum.B / count;
            return result;
        }
        public static LatencyTesterVM.RGBVal Sample3Channels( byte[] image, int width, int height)
        {
            LatencyTesterVM.RGBVal result = new LatencyTesterVM.RGBVal();
            LatencyTesterVM.RGBVal sum = new LatencyTesterVM.RGBVal();

            int count= 0;

           
            int size = width * height;
  
            for (int i = 0; i < size / 2; i += 3)
            {
                sum.R += image[i+2];
               // sum.G += (byte) image[i+1];
            //    sum.B += image[i];
                count++;
            }
            //TODO Stop assuming RGB format
            result.R = sum.R / count;
          //  result.G = sum.G / count;
        //    result.B = sum.B / count;
            return result;
        }
        
    }
}
