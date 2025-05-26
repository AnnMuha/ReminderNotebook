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

        private ReminderPriority? selectedPriorityFilter = null;
        private readonly IReminderRepository _repository;

        public ReminderPriority? SelectedPriorityFilter
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
        public ICommand ClearFiltersCommand { get; }
        public ICommand ShowReportCommand { get; }

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

        public MainViewModel(IReminderRepository repository)
        {
            _repository = repository;

            var loaded = _repository.Load();
            Reminders = new ObservableCollection<Reminder>(loaded);
            // Підписуємося на зміну IsCompleted для кожного нагадування
            foreach (var reminder in Reminders)
                SubscribeToReminder(reminder);

            AddCommand = new RelayCommand(AddReminder);
            DeleteCommand = new RelayCommand(DeleteReminder, () => SelectedReminder != null);
            EditCommand = new RelayCommand(EditReminder, () => SelectedReminder != null);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            ShowReportCommand = new RelayCommand(ShowReport);


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

            _repository.Save(Reminders.ToList());

        }

        private void ApplySearchAndFilter()
        {
            FilteredReminders.Clear();

            var result = Reminders.AsEnumerable();

            // Фільтр за пріоритетом
            if (SelectedPriorityFilter.HasValue)
            {
                result = result.Where(r => r.Priority == SelectedPriorityFilter.Value);
            }

            // Пошук по назві або опису
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                result = result.Where(r =>
                    r.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    r.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedStatusFilter == "Completed")
                result = result.Where(r => r.IsCompleted);

            else if (SelectedStatusFilter == "Pending")
                result = result.Where(r => !r.IsCompleted);

            // Сортування
            result = SelectedSortOption switch
            {
                "Newest first" => result.OrderByDescending(r => r.ReminderTime),
                "Oldest first" => result.OrderBy(r => r.ReminderTime),
                "By priority" => result.OrderByDescending(r => r.Priority),
                _ => result
            };

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
                SubscribeToReminder(window.NewReminder); // підписка тут
                _repository.Save(Reminders.ToList());
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
                    SubscribeToReminder(window.NewReminder); // знову підписка
                    SelectedReminder = window.NewReminder;

                    _repository.Save(Reminders.ToList());
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

            _repository.Save(Reminders.ToList());
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
        private string selectedSortOption = "Newest first";
        public string SelectedSortOption
        {
            get => selectedSortOption;
            set
            {
                selectedSortOption = value;
                OnPropertyChanged();
                ApplySearchAndFilter();
            }
        }

        public List<string> SortOptions { get; } = new List<string>
        {
            "Newest first",
            "Oldest first",
            "By priority"
        };

        public Array PriorityOptions => Enum.GetValues(typeof(ReminderPriority));

        private void ClearFilters()
        {
            SearchQuery = string.Empty;
            SelectedPriorityFilter = null;
            SelectedSortOption = "Newest first";
        }

        public List<string> StatusOptions { get; } = new List<string>
        {
            "All",
            "Completed",
            "Pending"
        };

        private string selectedStatusFilter = "All";
        public string SelectedStatusFilter
        {
            get => selectedStatusFilter;
            set
            {
                selectedStatusFilter = value;
                OnPropertyChanged();
                ApplySearchAndFilter();
            }
        }

        private void SaveData()
        {
            _repository.Save(Reminders.ToList());
        }

        private void SubscribeToReminder(Reminder reminder)
        {
            reminder.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Reminder.IsCompleted))
                {
                    _repository.Save(Reminders.ToList());
                    ApplySearchAndFilter();
                }
            };
        }

        private void ShowReport()
        {
            var total = Reminders.Count;
            var completed = Reminders.Count(r => r.IsCompleted);
            var pending = Reminders.Count(r => !r.IsCompleted);

            var reportWindow = new ReportWindow(total, completed, pending);
            reportWindow.ShowDialog();
        }

    }
}
