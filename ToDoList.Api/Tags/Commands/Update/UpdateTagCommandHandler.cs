using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Tags.Commands
{
    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, Tag>
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IValidator<UpdateTagCommand> _validator;
        public UpdateTagCommandHandler(IRepository<Tag> tagRepository, IValidator<UpdateTagCommand> validator)
        {
            _tagRepository = tagRepository;
            _validator = validator;
        }

        public async Task<Tag> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Tag tag = await _tagRepository.GetByIdAsync(request.Id);
            if (tag == null)
            {
                throw new Exception($"Tag with ID {request.Id} not found.");
            }
            var tags = new List<Tag>();
            tag.Name = request.Name;
            await _tagRepository.UpdateAsync(tag);
            return tag;
        }
    }
}
