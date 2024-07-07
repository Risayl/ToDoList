using MediatR;
using ToDoList.Models;
namespace ToDoList.Reminders.Queries
{
    public class GetReminderByIdQuery : IRequest<Reminder>
    {
        public int Id { get; set; }
    }
}