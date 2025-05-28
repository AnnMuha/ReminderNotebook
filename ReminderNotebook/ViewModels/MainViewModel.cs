using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using ReminderNotebook.Models;
using ReminderNotebook.Utils;
using ReminderNotebook.Views;
using ReminderNotebook.Services;
using ReminderNotebook.Filters;

namespace ReminderNotebook.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FilterService filterService;
        private readonly DispatcherTimer reminderTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
        private readonly List<IReminderObserver> observers = new();
        private readonly IReminderRepository _repository;

        public ObservableCollection<Reminder> Reminders { get; set; }
        public ObservableCollection<Reminder> FilteredReminders { get; set; } = new();

        private ReminderPriority? selectedPriorityFilter = null;

        private const string StatusAll = "All";
        private const string StatusCompleted = "Completed";
        private const string StatusPending = "Pending";
        private const string SortNewestFirst = "Newest first";

        private void AddReminderWithSubscription(Reminder reminder)
        {
            Reminders.Add(reminder);
            SubscribeToReminder(reminder);
        }

        public ReminderPriority? SelectedPriorityFilter
        {
            get => selectedPriorityFilter;
            set
            {
                selectedPriorityFilter = value;
                OnPropertyChanged();
                ApplyFiltersAndSort();
            }
        }

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

        private string searchQuery = string.Empty;
        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                OnPropertyChanged();
                ApplyFiltersAndSort();
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
                ApplyFiltersAndSort();
            }
        }

        private string selectedStatusFilter = "All";
        public string SelectedStatusFilter
        {
            get => selectedStatusFilter;
            set
            {
                selectedStatusFilter = value;
                OnPropertyChanged();
                ApplyFiltersAndSort();
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand ClearFiltersCommand { get; set; }
        public ICommand ShowReportCommand { get; set; }

        public List<string> SortOptions => SortingService.AvailableSortOptions.ToList();
        public Array PriorityOptions => Enum.GetValues(typeof(ReminderPriority));
        public List<string> StatusOptions { get; set; } = new List<string> { StatusAll, StatusCompleted, StatusPending }; 

        public MainViewModel(IReminderRepository repository)
        {
            filterService = new FilterService();
            _repository = repository;

            InitializeData();
            InitializeCommands();
            SetupTimer();
            SetupNotifications();
        }

        private void InitializeData()
        {
            var loaded = _repository.Load();
            Reminders = new ObservableCollection<Reminder>(loaded);

            foreach (var reminder in Reminders)
                SubscribeToReminder(reminder);

            ApplyFiltersAndSort();
        }

        private void InitializeCommands()
        {
            AddCommand = new RelayCommand(AddReminder);
            DeleteCommand = new RelayCommand(DeleteReminder, () => SelectedReminder != null);
            EditCommand = new RelayCommand(EditReminder, () => SelectedReminder != null);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            ShowReportCommand = new RelayCommand(ShowReport);
        }

        private void SetupTimer()
        {
            reminderTimer.Tick += CheckForDueReminders;
            reminderTimer.Start();
        }

        private void SetupNotifications()
        {
            var notifier = new Notifier();
            Subscribe(notifier);
        }

        private void ApplyFiltersAndSort()
        {
            FilteredReminders.Clear();
            filterService.ClearStrategies();
            BuildFilterStrategies();

            var filtered = filterService.ApplyFilters(Reminders);
            var sorted = SortingService.Sort(filtered, SelectedSortOption);

            foreach (var reminder in sorted)
                FilteredReminders.Add(reminder);
        }

        private void BuildFilterStrategies()
        {
            if (SelectedPriorityFilter.HasValue)
                filterService.AddStrategy(new PriorityFilterStrategy(SelectedPriorityFilter.Value));

            if (!string.IsNullOrWhiteSpace(SearchQuery))
                filterService.AddStrategy(new SearchFilterStrategy(SearchQuery));

            if (SelectedStatusFilter != StatusAll)
                filterService.AddStrategy(new StatusFilterStrategy(SelectedStatusFilter));
        }

        private void CheckForDueReminders(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            var dueReminders = Reminders.Where(r => !r.IsNotified && r.ReminderTime <= now).ToList();

            foreach (var reminder in dueReminders)
            {
                Notify(reminder);
                reminder.IsNotified = true;
            }

            if (dueReminders.Any())
                SaveData();
        }

        private void AddReminder()
        {
            var window = new AddReminderWindow();
            var result = window.ShowDialog();

            if (result == true && window.NewReminder != null)
            {
                AddReminderWithSubscription(window.NewReminder);
                _repository.Save(Reminders.ToList());
                ApplyFiltersAndSort();

            }
        }

        private void AddNewReminder(Reminder reminder)
        {
            AddReminderWithSubscription(reminder);
            SaveData();
            ApplyFiltersAndSort();

        }

        private void EditReminder()
        {
            if (SelectedReminder == null) return;

            var editedReminder = CreateReminderCopy(SelectedReminder);
            var window = new AddReminderWindow(editedReminder);
            var result = window.ShowDialog();

            if (result == true && window.NewReminder != null)
            {
                int index = Reminders.IndexOf(SelectedReminder);
                if (index >= 0)
                {
                    UnsubscribeFromReminder(Reminders[index]);

                    Reminders[index] = window.NewReminder;
                    SubscribeToReminder(window.NewReminder);
                    SelectedReminder = window.NewReminder;

                    _repository.Save(Reminders.ToList());
                    ApplyFiltersAndSort();

                }
            }
        }

        private Reminder CreateReminderCopy(Reminder original)
        {
            return new Reminder
            {
                Title = original.Title,
                Description = original.Description,
                ReminderTime = original.ReminderTime,
                Priority = original.Priority,
                CreatedAt = original.CreatedAt
            };
        }

        private void DeleteReminder()
        {
            if (SelectedReminder == null) return; 

            UnsubscribeFromReminder(SelectedReminder);
            Reminders.Remove(SelectedReminder);
            SelectedReminder = null;
            SaveData();
            ApplyFiltersAndSort();
        }

        private void ClearFilters()
        {
            SearchQuery = string.Empty;
            SelectedPriorityFilter = null;
            SelectedSortOption = "Newest first";
            SelectedStatusFilter = "All";
        }

        private void ShowReport()
        {
            var total = Reminders.Count;
            var completed = Reminders.Count(r => r.IsCompleted);
            var pending = total - completed;

            var reportWindow = new ReportWindow(total, completed, pending);
            reportWindow.ShowDialog();
        }

        private void SaveData()
        {
            _repository.Save(Reminders.ToList());
        }

        private void SubscribeToReminder(Reminder reminder)
        {
            reminder.PropertyChanged += OnReminderPropertyChanged;
        }
        private void UnsubscribeFromReminder(Reminder reminder)
        {
            reminder.PropertyChanged -= OnReminderPropertyChanged;
        }


        private void OnReminderPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Reminder.IsCompleted))
            {
                _repository.Save(Reminders.ToList());
                ApplyFiltersAndSort();
            }
        }

        public void Subscribe(IReminderObserver observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
        }

        public void Unsubscribe(IReminderObserver observer)
        {
            observers.Remove(observer);
        }

        private void Notify(Reminder reminder)
        {
            foreach (var observer in observers)
                observer.OnReminderTriggered(reminder);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
