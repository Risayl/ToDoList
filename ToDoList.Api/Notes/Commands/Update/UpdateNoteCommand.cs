using MediatR;
using ToDoList.Models;
namespace ToDoList.Notes.Commands
{
    public class UpdateNoteCommand : IRequest<Note>
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public List<string>? Tags { get; set; }
    }
}
