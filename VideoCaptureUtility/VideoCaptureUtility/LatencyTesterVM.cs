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
    public class LatencyTesterVM: INotifyPropertyChanged
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
       public  struct RGBVal
        {
          public  int r, b, g;
        }

        public enum BGColors
        {
            Black,
            White,
            Red,
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
        RGBVal colorThreshold;
        RGBVal colorAverage;
        Stopwatch stopwatch;
        int latencyRunningAvg;
        CaptureControlVM captureControl;
        #endregion
        public LatencyTesterVM()
        {
            //captureControl = control;
            m_colorList = new List<string>();
            m_colorList = Enum.GetNames(typeof(BGColors)).ToList();
            stopwatch = new Stopwatch();
            rectColor = Brushes.DarkGray;
            colorThreshold = new RGBVal();
            colorThreshold.r = 150;
            colorThreshold.g = 0;
            colorThreshold.b = 0;
            colorAverage = new RGBVal();
            colorAverage.r = 0;
            colorAverage.g = 0;
            colorAverage.b = 0;
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

        /// <summary>
        /// Runs a ColorSampler and updates the avreage on the GUI
        /// </summary>
        public void _sampleThread()
        {
           while(AlwaysSample)
           {
               RGBVal result= ColorSampler.Sample3Channels((BitmapSource)captureControl.Image);
               try
               {
                   Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() =>
                 {
                     ColorAverage = result;

                 }));
                 
               }
               catch (Exception e)
               {
                   Console.WriteLine(e);
               }
           }
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
            for (int i = 0; i < size; i+=3 )
            {
                sum.r += pixels[i];
                sum.b += pixels[i];
                sum.g += pixels[i];
                count++;
            }
                //TODO Stop assuming RGB format
            result.r = sum.r / count;
            result.b = sum.b / count;
            result.g = sum.g / count;
            return result;
        }
        public static LatencyTesterVM.RGBVal Sample3Channels( byte[] image, int width, int height)
        {
            LatencyTesterVM.RGBVal result = default(LatencyTesterVM.RGBVal);// = new LatencyTesterVM.RGBVal();
            LatencyTesterVM.RGBVal sum = default(LatencyTesterVM.RGBVal);// = new LatencyTesterVM.RGBVal();

            int count= 0;

           
            int size = width * height;
            byte[] pixels = new byte[size];
            for (int i = 0; i < size; i+=3 )
            {
                sum.r += pixels[i];
                sum.b += pixels[i];
                sum.g += pixels[i];
                count++;
            }
                //TODO Stop assuming RGB format
            result.r = sum.r / count;
            result.b = sum.b / count;
            result.g = sum.g / count;
            return result;
        }
        
    }
}
