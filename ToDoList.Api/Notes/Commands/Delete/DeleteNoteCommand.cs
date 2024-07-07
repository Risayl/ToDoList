using MediatR;
using ToDoList.Models;
namespace ToDoList.Notes.Commands
{
    public class DeleteNoteCommand : IRequest<Note>
    {
        public int Id { get; set; }
    }
}
