using System;
using System.Globalization;
using System.Windows.Data;
using WpfApp1.Models;

namespace WpfApp1.Converters
{
    /// <summary>
    /// Converter to display enum values as human-readable strings in the UI.
    /// Converts TaskStatus and TaskPriority enums to display-friendly text.
    /// </summary>
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                return enumValue.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && targetType.IsEnum)
            {
                return Enum.Parse(targetType, stringValue);
            }
            return null;
        }
    }

    /// <summary>
    /// Converter to change UI element colors based on task priority.
    /// </summary>
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority priority)
            {
                if (priority == TaskPriority.High)
                    return "#DC3545";      // Red
                if (priority == TaskPriority.Medium)
                    return "#FFC107";    // Orange
                if (priority == TaskPriority.Low)
                    return "#28A745";       // Green
            }
            return "#6C757D";                        // Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter to change UI element colors based on task status.
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskStatus status)
            {
                if (status == TaskStatus.Completed)
                    return "#28A745";     // Green
                if (status == TaskStatus.InProgress)
                    return "#007BFF";    // Blue
                if (status == TaskStatus.Pending)
                    return "#FFC107";       // Orange
            }
            return "#6C757D";                         // Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
