using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderNotebook.Models;
using ReminderNotebook.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using ReminderNotebook.Views;
using ReminderNotebook.Services;

namespace ReminderNotebook.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Reminder> Reminders { get; set; }
        public ObservableCollection<Reminder> FilteredReminders { get; set; } = new();

        private string selectedPriorityFilter = "All";
        public string SelectedPriorityFilter
        {
            get => selectedPriorityFilter;
            set
            {
                selectedPriorityFilter = value;
                OnPropertyChanged();
                ApplySearchAndFilter();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }

        private Reminder? selectedReminder;
        public Reminder? SelectedReminder
        {
            get => selectedReminder;
            set
            {
                selectedReminder = value;
                OnPropertyChanged();
            }
        }

        private readonly DispatcherTimer reminderTimer;
        private readonly List<IReminderObserver> observers = new();

        public MainViewModel()
        {
            var loaded = StorageService.Load();
            Reminders = new ObservableCollection<Reminder>(loaded);

            AddCommand = new RelayCommand(AddReminder);
            DeleteCommand = new RelayCommand(DeleteReminder, () => SelectedReminder != null);
            EditCommand = new RelayCommand(EditReminder, () => SelectedReminder != null);

            ApplySearchAndFilter();

            // Таймер для сповіщень
            reminderTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();

            // Підписуємо Notifier як спостерігача
            var notifier = new Notifier();
            Subscribe(notifier);
        }

        private void ReminderTimer_Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;

            foreach (var reminder in Reminders)
            {
                if (!reminder.IsNotified && reminder.ReminderTime <= now)
                {
                    Notify(reminder);
                    reminder.IsNotified = true;
                }
            }

            StorageService.Save(Reminders.ToList());
        }

        private void ApplySearchAndFilter()
        {
            FilteredReminders.Clear();

            var result = Reminders.AsEnumerable();

            // 🔹 Фільтр по пріоритету
            if (SelectedPriorityFilter != "All")
            {
                result = result.Where(r => r.Priority.ToString() == SelectedPriorityFilter);
            }

            // 🔹 Пошук по назві або опису
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                result = result.Where(r =>
                    r.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    r.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var reminder in result)
                FilteredReminders.Add(reminder);
        }


        private void AddReminder()
        {
            var window = new AddReminderWindow();
            bool? result = window.ShowDialog();

            if (result == true && window.NewReminder != null)
            {
                Reminders.Add(window.NewReminder);
                StorageService.Save(Reminders.ToList());
                ApplySearchAndFilter();
            }
        }

        private void EditReminder()
        {
            if (SelectedReminder == null) return;

            var editedReminder = new Reminder
            {
                Title = SelectedReminder.Title,
                Description = SelectedReminder.Description,
                ReminderTime = SelectedReminder.ReminderTime,
                Priority = SelectedReminder.Priority,
                CreatedAt = SelectedReminder.CreatedAt
            };

            var window = new AddReminderWindow(editedReminder);
            bool? result = window.ShowDialog();

            if (result == true && window.NewReminder != null)
            {
                int index = Reminders.IndexOf(SelectedReminder);
                if (index >= 0)
                {
                    Reminders[index] = window.NewReminder;
                    SelectedReminder = window.NewReminder;

                    StorageService.Save(Reminders.ToList());
                    ApplySearchAndFilter();
                }
            }
        }

        private void DeleteReminder()
        {
            if (SelectedReminder == null)
                return;

            Reminders.Remove(SelectedReminder);
            SelectedReminder = null;

            StorageService.Save(Reminders.ToList());
            ApplySearchAndFilter();
        }

        public void Subscribe(IReminderObserver observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
        }

        public void Unsubscribe(IReminderObserver observer)
        {
            if (observers.Contains(observer))
                observers.Remove(observer);
        }

        private void Notify(Reminder reminder)
        {
            foreach (var observer in observers)
            {
                observer.OnReminderTriggered(reminder);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string searchQuery = string.Empty;
        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                OnPropertyChanged();
                ApplySearchAndFilter();
            }
        }

    }
}
