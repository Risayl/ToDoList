using MediatR;
using ToDoList.Models;
namespace ToDoList.Reminders.Commands
{
    public class DeleteReminderCommand : IRequest<Reminder>
    {
        public int Id { get; set; }
    }
}
