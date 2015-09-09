using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OculusFPV.Launcher.ValueConverters
{

    // This ValueConverter converts an Enum value to its corresponding string value.

    // Sample XAML code that uses this converter:
    // Text="{Binding Pgm_State,
    //  Converter={StaticResource EnumToStringConverter}}"

    // In the example, Pgm_State is a property that has an Enum type of "PgmRunStateValue".

    public sealed class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string EnumString;
            try
            {
                EnumString = Enum.GetName((value.GetType()), value);
                return EnumString;
            }
            catch
            {
                return string.Empty;
            }
        }


        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
             if(targetType == typeof(VIWrapper.PhysicalType))
             {
                 VIWrapper.PhysicalType physType;
                 Enum.TryParse<VIWrapper.PhysicalType>((String)value, out physType);
                 return physType;
             }
             if (targetType == typeof(VIWrapper.MSubType))
             {
                 VIWrapper.MSubType physType;
                 Enum.TryParse<VIWrapper.MSubType>((String)value, out physType);
                 return physType;
             }
             if (targetType == typeof(VIWrapper.FormatType))
             {
                 VIWrapper.FormatType physType;
                 Enum.TryParse<VIWrapper.FormatType>((String)value, out physType);
                 return physType;
             }
            throw new NotImplementedException();
        }
    }
    
}
