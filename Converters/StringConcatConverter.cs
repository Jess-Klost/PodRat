using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RSSPod;

public class StringConcatConverter : IValueConverter
{
    public static readonly StringConcatConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, 
                                                            CultureInfo culture)
    {
        if (value is string s1)
        {
            if (parameter is string s2)
            {
                return s1 + s2;
            }
            return s1;
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