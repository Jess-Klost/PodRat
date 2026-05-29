using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RSSPod;

public class HTMLConverter : IValueConverter
{
    public static readonly HTMLConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, 
                                                            CultureInfo culture)
    {
        if (value is string html && targetType.IsAssignableTo(typeof(string)))
        {
            string output = html.Replace("<p>", "");
            output = output.Replace("</p>", "\n");
            output = Regex.Replace(output, "<.*?>", "");
            return output;
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