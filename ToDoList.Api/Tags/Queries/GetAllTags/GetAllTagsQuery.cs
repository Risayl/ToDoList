using MediatR;
using ToDoList.Models;

namespace ToDoList.Tags.Queries
{
    public class GetAllTagsQuery : IRequest<IEnumerable<Tag>>
    {
    }
}