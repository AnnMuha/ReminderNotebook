using System.Collections.Generic;
using System.Linq;
using ReminderNotebook.Filters;
using ReminderNotebook.Models;

namespace ReminderNotebook.Services
{
    public class FilterService
    {
        private readonly List<IFilterStrategy> activeStrategies = new();

        public IReadOnlyList<IFilterStrategy> ActiveStrategies => activeStrategies.AsReadOnly();

        public void AddStrategy(IFilterStrategy strategy)
        {
            if (strategy != null && !activeStrategies.Contains(strategy))
            {
                activeStrategies.Add(strategy);
            }
        }

        public void RemoveStrategy(IFilterStrategy strategy)
        {
            activeStrategies.Remove(strategy);
        }

        public void ClearStrategies()
        {
            activeStrategies.Clear();
        }

        public IEnumerable<Reminder> ApplyFilters(IEnumerable<Reminder> reminders)
        {
            return activeStrategies.Aggregate(reminders,
                (current, strategy) => strategy.Apply(current));
        }

        public bool HasActiveFilters => activeStrategies.Count > 0;

        public string GetActiveFiltersDescription()
        {
            if (!HasActiveFilters)
                return "No filters applied";

            return string.Join(", ", activeStrategies.Select(s => s.Description));
        }
    }
}