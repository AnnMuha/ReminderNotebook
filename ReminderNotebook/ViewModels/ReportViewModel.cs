using ReminderNotebook.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ReminderNotebook.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        public int TotalReminders { get; }
        public int CompletedReminders { get; }
        public int PendingReminders { get; }

        public Dictionary<ReminderPriority, int> PriorityCounts { get; }

        public ReportViewModel(IEnumerable<Reminder> reminders)
        {
            TotalReminders = reminders.Count();
            CompletedReminders = reminders.Count(r => r.IsCompleted);
            PendingReminders = reminders.Count(r => !r.IsCompleted);

            PriorityCounts = reminders
                .GroupBy(r => r.Priority)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }



    }
}

