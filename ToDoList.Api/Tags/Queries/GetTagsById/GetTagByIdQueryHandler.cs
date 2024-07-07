using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Tags.Queries
{
    public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, Tag>
    {
        private readonly IRepository<Tag> _tagRepository;

        public GetTagByIdQueryHandler(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<Tag> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            Tag tag = await _tagRepository.GetByIdAsync(request.Id);
            if (tag == null)
            {
                throw new Exception($"Tag with ID {request.Id} not found.");
            }
            return tag;
        }
    }
}
