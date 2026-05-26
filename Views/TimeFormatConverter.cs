using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RSSPod;

public class TimeFormatConverter : IMultiValueConverter
{
    public static readonly TimeFormatConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 1
            && values[0] is long length)
        {
            if (values.Count >= 2
                && values[1] is double position)
            {
                length = (long)(length * position);
            }
            TimeSpan t = TimeSpan.FromMilliseconds(length);
            return t.ToString(@"hh\:mm\:ss");
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