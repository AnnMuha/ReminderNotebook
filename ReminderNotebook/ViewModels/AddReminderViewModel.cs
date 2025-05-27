using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using ReminderNotebook.Models;
using ReminderNotebook.Utils;

namespace ReminderNotebook.ViewModels
{
    public class AddReminderViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now.Date;
        public string Time { get; set; } = "12:00";
        public int SelectedPriorityIndex { get; set; } = 1; // Medium by default
        public Array CategoryOptions => Enum.GetValues(typeof(ReminderCategory));
        
        private ReminderCategory selectedCategory = ReminderCategory.General;
        public ReminderCategory SelectedCategory 
        { 
            get => selectedCategory;
            set 
            {
                selectedCategory = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<Reminder>? ReminderSaved;
        public event Action? CancelRequested;

        public AddReminderViewModel()
        {
            SaveCommand = new RelayCommand(SaveReminder);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SaveReminder()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Заповніть назву.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(Time, out var timeSpan))
            {
                MessageBox.Show("Невірний формат часу. Приклад: 14:30", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var priority = SelectedPriorityIndex switch
            {
                0 => ReminderPriority.Low,
                2 => ReminderPriority.High,
                _ => ReminderPriority.Medium
            };

            var reminder = new Reminder
            {
                Title = Title.Trim(),
                Description = Description.Trim(),
                ReminderTime = Date + timeSpan,
                Priority = priority,
                CreatedAt = DateTime.Now,
                Category = SelectedCategory
            };

            ReminderSaved?.Invoke(reminder);
        }

        private void Cancel() => CancelRequested?.Invoke();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
