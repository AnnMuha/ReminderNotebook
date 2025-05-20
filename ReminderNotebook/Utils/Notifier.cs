using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReminderNotebook.Models;

namespace ReminderNotebook.Utils
{
    public class Notifier : IReminderObserver
    {
        public void OnReminderTriggered(Reminder reminder)
        {
            MessageBox.Show($"⏰ Нагадування: {reminder.Title}", "Нагадування", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

