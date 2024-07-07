using MediatR;
using ToDoList.Models;
namespace ToDoList.Tags.Queries
{
    public class GetTagByIdQuery : IRequest<Tag>
    {
        public int Id { get; set; }
    }
}