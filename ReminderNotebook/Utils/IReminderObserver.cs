using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderNotebook.Models;

namespace ReminderNotebook.Utils
{
    public interface IReminderObserver
    {
        void OnReminderTriggered(Reminder reminder);
    }
}

