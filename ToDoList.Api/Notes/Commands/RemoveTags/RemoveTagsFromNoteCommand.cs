using MediatR;
using ToDoList.Models;
namespace ToDoList.Notes.Commands
{
    public class RemoveTagsFromNoteCommand : IRequest<Note>
    {
        public int Id { get; set; }
        public int TagId { get; set; }
    }
}
