using System;
using System.Collections.Generic;
using System.Linq;
using ReminderNotebook.Models;

namespace ReminderNotebook.Filters
{
    public class SearchFilterStrategy : IFilterStrategy
    {
        private readonly string searchQuery;

        public SearchFilterStrategy(string searchQuery)
        {
            this.searchQuery = searchQuery?.Trim() ?? string.Empty;
        }

        public string Description => $"Search: '{searchQuery}'";

        public IEnumerable<Reminder> Apply(IEnumerable<Reminder> reminders)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return reminders;

            return reminders.Where(r =>
                r.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                r.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
        }
    }
}