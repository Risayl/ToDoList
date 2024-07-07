using FluentValidation;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Notes.Commands
{
    public class AddTagsToNoteCommandValidator : AbstractValidator<AddTagsToNoteCommand>
    {
        public AddTagsToNoteCommandValidator() 
        {
            RuleFor(x => x.Id)
                 .NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Tags)
                 .NotEmpty().WithMessage("Tags are required.");
        }
    }
}
