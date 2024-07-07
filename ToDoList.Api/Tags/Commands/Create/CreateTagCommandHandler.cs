using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Tags.Commands
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Tag>
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IValidator<CreateTagCommand> _validator;
        public CreateTagCommandHandler(IRepository<Tag> tagRepository, IValidator<CreateTagCommand> validator)
        {
            _tagRepository = tagRepository;
            _validator = validator;
        }

        public async Task<Tag> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Tag tag = await _tagRepository.AddAsync(new Tag { Name = request.Name});
            return tag;
        }
    }
}
