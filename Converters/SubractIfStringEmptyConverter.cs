using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace PodRat;

public class SubtractIfStringEmptyConverter : IValueConverter
{
    public static readonly StringConcatConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, 
                                                            CultureInfo culture)
    {
        if (value is string valueString && parameter is string parameterString)
        {
            if(int.TryParse(parameterString, out int number))
            {
              if (string.IsNullOrEmpty(valueString))
                return number - 1;
                else 
                    return number;  
            }
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