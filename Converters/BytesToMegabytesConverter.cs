using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace PodBat;

public class BytesToMegabytesConverter : IValueConverter
{
    public static readonly BytesToMegabytesConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, 
                                                            CultureInfo culture)
    {
        if (value is long bytes)
        {
            return bytes / 1000000;
        }
        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), 
                                                BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, 
                                object? parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
}