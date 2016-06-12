using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MouseChef.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter as string;
            var invert = parameterString != null && parameterString.Equals("NOT", StringComparison.OrdinalIgnoreCase);
            return invert != (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
