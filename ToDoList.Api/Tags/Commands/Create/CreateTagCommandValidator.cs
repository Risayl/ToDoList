using FluentValidation;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Tags.Commands
{
    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        private readonly ITagRepository _tagRepository;
        public CreateTagCommandValidator(ITagRepository tagRepository) 
        {
            _tagRepository = tagRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tag name is required.")
                .MustAsync(BeUniqueTitle).WithMessage("Tag already exists.");
        }
        private async Task<bool> BeUniqueTitle(string Name, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetByNameAsync(Name);         
            if(tag != null) 
            {
                return false;
            }
            return true;
        }
    }
}
