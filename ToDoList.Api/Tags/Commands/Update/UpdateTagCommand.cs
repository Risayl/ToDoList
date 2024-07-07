using MediatR;
using ToDoList.Models;
namespace ToDoList.Tags.Commands
{
    public class UpdateTagCommand : IRequest<Tag>
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
