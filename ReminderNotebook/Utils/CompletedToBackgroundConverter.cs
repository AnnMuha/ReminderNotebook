using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReminderNotebook.Utils
{
    public class CompletedToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted && isCompleted)
            {
                return new SolidColorBrush(Color.FromRgb(200, 255, 200)); // світло-зелений
            }

            return new SolidColorBrush(Color.FromRgb(236, 240, 241)); // стандартний (як було)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

