using MediatR;
using ToDoList.Models;
namespace ToDoList.Tags.Commands
{
    public class CreateTagCommand : IRequest<Tag>
    {
        public string? Name { get; set; }
    }
}
