using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using PodRat.Models;

namespace PodRat;

public class NotDownloadedStatusBoolConverter : IValueConverter
{
    public static readonly NotDownloadedStatusBoolConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, 
                                                            CultureInfo culture)
    {
        if (value is DownloadManager.DownloadStatus downloadStatus)
        {
            return downloadStatus != DownloadManager.DownloadStatus.downloaded;
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