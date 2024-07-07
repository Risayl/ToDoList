using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Tags.Commands
{
    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Tag>
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IValidator<DeleteTagCommand> _validator;
        public DeleteTagCommandHandler(IRepository<Tag> tagRepository, IValidator<DeleteTagCommand> validator)
        {
            _tagRepository = tagRepository;
            _validator = validator;
        }

        public async Task<Tag> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Tag tag = await _tagRepository.GetByIdAsync(request.Id);
            if (tag == null)
            {
                throw new Exception($"Tag with ID {request.Id} not found.");
            }
            await _tagRepository.DeleteAsync(request.Id);
            return tag;
        }
    }
}
