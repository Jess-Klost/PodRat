using System;
using System.Collections;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RSSPod;

public class CollectionHasElements : IValueConverter
{
    public static readonly CollectionHasElements Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, 
                                                            CultureInfo culture)
    {
        if (value == null)
            return false;
        if (value is ICollection collection)
        {
            return collection.Count != 0;
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