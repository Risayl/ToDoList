using MediatR;
using ToDoList.Models;
namespace ToDoList.Reminders.Queries
{
    public class GetAllRemindersQuery : IRequest<IEnumerable<Reminder>>
    {
    }
}