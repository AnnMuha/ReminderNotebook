using System.Collections.Generic;
using ReminderNotebook.Models;

namespace ReminderNotebook.Services
{
    public interface IReminderRepository
    {
        void Save(List<Reminder> reminders);
        List<Reminder> Load();
    }
}
