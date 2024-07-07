using MediatR;
using ToDoList.Models;
namespace ToDoList.Tags.Commands
{
    public class DeleteTagCommand : IRequest<Tag>
    {
        public int Id { get; set; }
    }
}
