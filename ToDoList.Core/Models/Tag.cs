using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ToDoList.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public ICollection<Note>? Notes { get; set; } = new List<Note>();
        [JsonIgnore]
        public ICollection<Reminder>? Reminders { get; set; } = new List<Reminder>();
    }
}
