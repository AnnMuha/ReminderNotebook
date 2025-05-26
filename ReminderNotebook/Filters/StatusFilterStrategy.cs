using System.Collections.Generic;
using System.Linq;
using ReminderNotebook.Models;

namespace ReminderNotebook.Filters
{
    public class StatusFilterStrategy : IFilterStrategy
    {
        private readonly string status;

        public StatusFilterStrategy(string status)
        {
            this.status = status;
        }

        public string Description => $"Status: {status}";

        public IEnumerable<Reminder> Apply(IEnumerable<Reminder> reminders)
        {
            return status switch
            {
                "Completed" => reminders.Where(r => r.IsCompleted),
                "Pending" => reminders.Where(r => !r.IsCompleted),
                _ => reminders
            };
        }
    }
}