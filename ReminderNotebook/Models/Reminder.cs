using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReminderNotebook.Models
{
    public class Reminder : Note, INotifyPropertyChanged
    {
        private bool isCompleted;
        private ReminderCategory category = ReminderCategory.General;

        public DateTime ReminderTime { get; set; }
        public ReminderPriority Priority { get; set; } = ReminderPriority.Medium;
        public bool IsNotified { get; set; }
        public bool IsTriggered { get; set; } = false;
        
        public ReminderCategory Category
        {
            get => category;
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                if (isCompleted != value)
                {
                    isCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public enum ReminderPriority
    {
        Low,
        Medium,
        High
    }
    
    public enum ReminderCategory
    {
        General,
        Work,
        Personal,
        Health,
        Shopping,
        Bills,
        Education,
        Events,
        Travel,
        Other
    }
}
