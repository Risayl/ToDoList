using MediatR;
using ToDoList.Models;
namespace ToDoList.Notes.Commands
{
    public class CreateNoteCommand : IRequest<Note>
    {
        public string? Title { get; set; }
        public string? Text { get; set; }
        public List<string>? Tags { get; set; }
    }
}
