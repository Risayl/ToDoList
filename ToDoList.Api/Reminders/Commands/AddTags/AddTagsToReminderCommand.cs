using MediatR;
using ToDoList.Models;
namespace ToDoList.Reminders.Commands
{
    public class AddTagsToReminderCommand : IRequest<Reminder>
    {
        public int Id { get; set; }
        public List<string>? Tags { get; set; }
    }
}
