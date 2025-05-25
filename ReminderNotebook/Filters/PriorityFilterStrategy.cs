using System;
using System.Collections.Generic;
using System.Linq;
using ReminderNotebook.Models;

namespace ReminderNotebook.Filters
{
    public class PriorityFilterStrategy : IFilterStrategy
    {
        private readonly ReminderPriority priority;

        public PriorityFilterStrategy(ReminderPriority priority)
        {
            this.priority = priority;
        }

        public string Description => $"Priority: {priority}";

        public IEnumerable<Reminder> Apply(IEnumerable<Reminder> reminders)
        {
            return reminders.Where(r => r.Priority == priority);
        }
    }
}