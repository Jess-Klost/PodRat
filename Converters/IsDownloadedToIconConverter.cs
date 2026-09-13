using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace PodRat;

public class IsDownloadedToIconConverter : IValueConverter
{
    public static readonly IsDownloadedToIconConverter Instance = new();
    public static object downloadIcon;
    public static object playIcon;

    public object? Convert(object? value, Type targetType, object? parameter, 
                                                            CultureInfo culture)
    {
        if (value is bool downloaded)
        {
            if (downloaded)
                return playIcon;
            else
                return downloadIcon;
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