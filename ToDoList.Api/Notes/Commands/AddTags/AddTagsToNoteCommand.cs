using MediatR;
using ToDoList.Models;
namespace ToDoList.Notes.Commands
{
    public class AddTagsToNoteCommand : IRequest<Note>
    {
        public int Id { get; set; }
        public List<string>? Tags { get; set; }
    }
}
