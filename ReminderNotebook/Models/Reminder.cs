using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderNotebook.Models
{
    public class Reminder : Note
    {
        public DateTime ReminderTime { get; set; }
        public ReminderPriority Priority { get; set; } = ReminderPriority.Medium;
        public bool IsNotified { get; set; }
        public bool IsTriggered { get; set; } = false;
    }

    public enum ReminderPriority
    {
        Low,
        Medium,
        High
    }
}
