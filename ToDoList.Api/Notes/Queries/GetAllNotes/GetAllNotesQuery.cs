using MediatR;
using ToDoList.Models;

namespace ToDoList.Notes.Queries
{
    public class GetAllNotesQuery : IRequest<IEnumerable<Note>>
    {
    }
}