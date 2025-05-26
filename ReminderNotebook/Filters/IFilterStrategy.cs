using System.Collections.Generic;
using ReminderNotebook.Models;

namespace ReminderNotebook.Filters
{
    public interface IFilterStrategy
    {
        IEnumerable<Reminder> Apply(IEnumerable<Reminder> reminders);
        string Description { get; }
    }
}