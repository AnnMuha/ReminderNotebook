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
            Reminders = new ObservableCollection<Reminder>();

            AddCommand = new RelayCommand(AddReminder);
            DeleteCommand = new RelayCommand(DeleteReminder, () => SelectedReminder != null);
            EditCommand = new RelayCommand(EditReminder, () => SelectedReminder != null);
        }

        private void DeleteReminder()
        {
            if (SelectedReminder != null)
            {
                Reminders.Remove(SelectedReminder);
            }
        }

        private void EditReminder()
        {
            // TODO: Відкрити AddReminderWindow з переданим SelectedReminder
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
            }
        }

    }
}

