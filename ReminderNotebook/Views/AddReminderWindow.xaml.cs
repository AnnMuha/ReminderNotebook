using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReminderNotebook.Models;
using ReminderNotebook.ViewModels;


namespace ReminderNotebook.Views
{
    public partial class AddReminderWindow : Window
    {
        public Reminder? NewReminder { get; private set; }

        public AddReminderWindow()
        {
            InitializeComponent();
            DataContext = new AddReminderViewModel();

            var vm = new AddReminderViewModel();
            vm.ReminderSaved += reminder =>
            {
                NewReminder = reminder;
                DialogResult = true;
                Close();
            };

            vm.CancelRequested += () =>
            {
                DialogResult = false;
                Close();
            };

            DataContext = vm;
        }

        public AddReminderWindow(Reminder reminderToEdit) : this()
        {
            if (DataContext is AddReminderViewModel vm)
            {
                vm.Title = reminderToEdit.Title;
                vm.Description = reminderToEdit.Description;
                vm.Date = reminderToEdit.ReminderTime.Date;
                vm.Time = reminderToEdit.ReminderTime.ToString("HH:mm");
                vm.SelectedPriorityIndex = reminderToEdit.Priority switch
                {
                    ReminderPriority.Low => 0,
                    ReminderPriority.Medium => 1,
                    ReminderPriority.High => 2,
                    _ => 1
                };

                NewReminder = reminderToEdit;
            }
        }
    }
}
