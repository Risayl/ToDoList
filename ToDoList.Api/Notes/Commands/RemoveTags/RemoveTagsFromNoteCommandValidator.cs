using FluentValidation;

namespace ToDoList.Notes.Commands
{
    public class RemoveTagsFromNoteCommandValidator : AbstractValidator<RemoveTagsFromNoteCommand>
    {
        public RemoveTagsFromNoteCommandValidator() 
        {
            RuleFor(x => x.Id)
                 .NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.TagId)
                 .NotEmpty().WithMessage("Tag Id is required.");
        }
    }
}
