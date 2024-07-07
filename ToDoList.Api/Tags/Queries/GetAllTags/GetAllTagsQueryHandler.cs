using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Tags.Queries
{
    public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, IEnumerable<Tag>>
    {
        private readonly IRepository<Tag> _tagRepository;

        public GetAllTagsQueryHandler(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<IEnumerable<Tag>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags;
        }
    }
}
