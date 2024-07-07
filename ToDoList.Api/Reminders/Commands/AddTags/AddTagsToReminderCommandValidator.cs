using FluentValidation;

namespace ToDoList.Reminders.Commands
{
    public class AddTagsToReminderCommandValidator : AbstractValidator<AddTagsToReminderCommand>
    {
        public AddTagsToReminderCommandValidator() 
        {
            RuleFor(x => x.Id)
                 .NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Tags)
                 .NotEmpty().WithMessage("Tags are required.");
        }
    }
}
