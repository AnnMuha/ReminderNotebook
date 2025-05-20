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
using ReminderNotebook.Views;
using ReminderNotebook.Services;
using System.Linq;


namespace ReminderNotebook.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Reminder> Reminders { get; set; }

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

        public MainViewModel()
        {
            var loaded = StorageService.Load();
            Reminders = new ObservableCollection<Reminder>();

            AddCommand = new RelayCommand(AddReminder);
            DeleteCommand = new RelayCommand(DeleteReminder, () => SelectedReminder != null);
            EditCommand = new RelayCommand(EditReminder, () => SelectedReminder != null);
        }

        private void DeleteReminder()
        {
            if (SelectedReminder == null)
                return;

            Reminders.Remove(SelectedReminder);
            SelectedReminder = null;

            StorageService.Save(Reminders.ToList());
        }

        private void EditReminder()
        {
            if (SelectedReminder == null) return;

            // Створюємо копію, щоб не змінювати напряму
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
                // Замінюємо старий Reminder на оновлений
                int index = Reminders.IndexOf(SelectedReminder);
                if (index >= 0)
                {
                    Reminders[index] = window.NewReminder;
                    SelectedReminder = window.NewReminder;

                    StorageService.Save(Reminders.ToList());
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void AddReminder()
        {
            var window = new Views.AddReminderWindow();
            bool? result = window.ShowDialog();

            if (result == true && window.NewReminder != null)
            {
                Reminders.Add(window.NewReminder);
                StorageService.Save(Reminders.ToList());
            }
        }

    }
}

