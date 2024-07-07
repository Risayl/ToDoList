using MediatR;
using ToDoList.Models;

namespace ToDoList.Notes.Queries
{
    public class GetNoteByIdQuery : IRequest<Note>
    {
        public int Id { get; set; }
    }
}