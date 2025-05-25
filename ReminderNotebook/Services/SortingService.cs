using System;
using System.Collections.Generic;
using System.Linq;
using ReminderNotebook.Models;

namespace ReminderNotebook.Services
{
    public class SortingService
    {
        public static IEnumerable<Reminder> Sort(IEnumerable<Reminder> reminders, string sortOption)
        {
            return sortOption switch
            {
                "Newest first" => reminders.OrderByDescending(r => r.ReminderTime),
                "Oldest first" => reminders.OrderBy(r => r.ReminderTime),
                "By priority" => reminders.OrderByDescending(r => r.Priority)
                                        .ThenBy(r => r.ReminderTime),
                "By title" => reminders.OrderBy(r => r.Title),
                _ => reminders
            };
        }

        public static readonly string[] AvailableSortOptions = {
            "Newest first",
            "Oldest first",
            "By priority",
            "By title"
        };
    }
}