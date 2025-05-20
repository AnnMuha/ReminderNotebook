using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using ReminderNotebook.Models;

namespace ReminderNotebook.Services
{
    public static class StorageService
    {
        private static readonly string filePath = "reminders.json";

        public static void Save(List<Reminder> reminders)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(reminders, options);
            File.WriteAllText(filePath, json);
        }

        public static List<Reminder> Load()
        {
            if (!File.Exists(filePath))
                return new List<Reminder>();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Reminder>>(json) ?? new List<Reminder>();
        }
    }
}
