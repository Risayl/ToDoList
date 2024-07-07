using FluentValidation;

namespace ToDoList.Reminders.Commands
{
    public class RemoveTagsFromReminderCommandValidator : AbstractValidator<RemoveTagsFromReminderCommand>
    {
        public RemoveTagsFromReminderCommandValidator() 
        {
            RuleFor(x => x.Id)
                 .NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.TagId)
                 .NotEmpty().WithMessage("Tag Id is required.");
        }
    }
}
