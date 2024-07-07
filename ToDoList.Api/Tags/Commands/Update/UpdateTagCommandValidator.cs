using FluentValidation;
using System.Xml.Linq;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Tags.Commands
{
    public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
    {
        private readonly ITagRepository _tagRepository;
        public UpdateTagCommandValidator(ITagRepository tagRepository) 
        {
            _tagRepository = tagRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tag name is required.")
                .MustAsync(BeUniqueTitle).WithMessage("Tag already exists.");

        }
        private async Task<bool> BeUniqueTitle(string Name, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetByNameAsync(Name);
            if (tag != null)
            {
                return false;
            }
            return true;
        } 
    }
}
