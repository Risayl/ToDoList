using MediatR;
using ToDoList.Models;
namespace ToDoList.Reminders.Commands
{
    public class CreateReminderCommand : IRequest<Reminder>
    {
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string ReminderTime { get; set; }
        public List<string>? Tags { get; set; }
    }
}
