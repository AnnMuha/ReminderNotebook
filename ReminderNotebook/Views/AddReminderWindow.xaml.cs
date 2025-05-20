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


namespace ReminderNotebook.Views
{
    /// <summary>
    /// Interaction logic for AddReminderWindow.xaml
    /// </summary>
    public partial class AddReminderWindow : Window
    {
        public Reminder NewReminder { get; private set; }

        public AddReminderWindow()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Now.Date;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заповніть назву та дату.");
                return;
            }

            if (!TimeSpan.TryParse(TimeTextBox.Text, out TimeSpan time))
            {
                MessageBox.Show("Невірний формат часу. Приклад: 14:30");
                return;
            }

            var dateTime = DatePicker.SelectedDate.Value.Date + time;

            ReminderPriority priority = ReminderPriority.Medium;
            if (PriorityComboBox.SelectedIndex == 0) priority = ReminderPriority.Low;
            else if (PriorityComboBox.SelectedIndex == 2) priority = ReminderPriority.High;

            NewReminder = new Reminder
            {
                Title = TitleTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                ReminderTime = dateTime,
                Priority = priority,
                CreatedAt = DateTime.Now
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
