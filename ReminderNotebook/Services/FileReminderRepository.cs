using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ReminderNotebook.Models;

namespace ReminderNotebook.Services
{
    public class FileReminderRepository : IReminderRepository
    {
        private readonly string filePath;

        public FileReminderRepository(string path = "reminders.json")
        {
            filePath = path;
        }

        public void Save(List<Reminder> reminders)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(reminders, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new IOException("Помилка під час збереження нагадувань", ex);
            }
        }

        public List<Reminder> Load()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<Reminder>();

                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Reminder>>(json) ?? new List<Reminder>();
            }
            catch (Exception ex)
            {
                throw new IOException("Помилка під час завантаження нагадувань", ex);
            }
        }
    }
}
