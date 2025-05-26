using System;
using System.Windows;
using ReminderNotebook.Services;
using ReminderNotebook.ViewModels;
using ReminderNotebook.Views;

namespace ReminderNotebook
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var repository = new FileReminderRepository(); // або інший репозиторій
            var mainViewModel = new MainViewModel(repository);
            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();
        }
    }
}
