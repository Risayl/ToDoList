using MediatR;
using ToDoList.Models;
namespace ToDoList.Reminders.Commands
{
    public class RemoveTagsFromReminderCommand : IRequest<Reminder>
    {
        public int Id { get; set; }
        public int TagId { get; set; }
    }
}
