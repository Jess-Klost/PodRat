using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RSSPod;

public class DateTimeFullDateConverter : IValueConverter
{
    public static readonly DateTimeFullDateConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, 
                                                            CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString(culture.DateTimeFormat.FullDateTimePattern);
        }
        else if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToLocalTime().ToString(culture.DateTimeFormat.FullDateTimePattern);
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