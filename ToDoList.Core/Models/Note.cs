using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ToDoList.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Text { get; set; }
        public ICollection<Tag>? Tags { get; set; }
    }
}
