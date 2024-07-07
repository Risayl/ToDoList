using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Text { get; set; }
        public DateTime ReminderTime { get; set; }
        public ICollection<Tag>? Tags { get; set; }
    }
}
