using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ReminderNotebook.Models;

namespace ReminderNotebook.Utils
{
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReminderPriority priority)
            {
                return priority switch
                {
                    ReminderPriority.Low => Brushes.Green,
                    ReminderPriority.Medium => Brushes.Orange,
                    ReminderPriority.High => Brushes.Red,
                    _ => Brushes.Gray
                };
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

