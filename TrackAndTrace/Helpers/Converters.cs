using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace TrackAndTrace.Helpers
{
    public class StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (parameter.ToString())
            {
                case "JM":
                    value = "Jedinica mjere: " + value.ToString();
                    break;
                case "G":
                    value = value.ToString() + ",  ";
                    break;
                case "BPM":
                    value = "Broj proizvodnog mjesta: (" + value.ToString() + ")";
                    break;
                default:
                    break;
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                DateTime dateTime = (DateTime)value;
                if (dateTime.Year != 1)
                    return new DateTimeOffset(dateTime);
                else
                    return DateTimeOffset.Now;
            }
            catch (Exception e)
            {
                return DateTimeOffset.Now;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                return dateTimeOffset.DateTime;
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }
        }
    }

    public class BoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                Int32 v = (Int32)value;
                return (v == 0) ? false : true;
                
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                bool v = (bool)value;
                return !v ? 0 : 1;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }

    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                DateTime dateTime = (DateTime)value;
                return dateTime.ToString("dd.MM.yyyy");
            }
            catch (Exception e)
            {
                return DateTime.Now.ToString("dd.MM.yyyy");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public static class Converters
    {
        public static bool IsNotNull(object value) => value != null;
        public static bool IsNull(object value) => value == null;
    }
}
