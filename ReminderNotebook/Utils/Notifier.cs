using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReminderNotebook.Utils
{
    public static class Notifier
    {
        public static void Show(string message)
        {
            MessageBox.Show(message, "Нагадування", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
