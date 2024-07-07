using FluentValidation;

namespace ToDoList.Reminders.Commands
{
    public class DeleteReminderCommandValidator : AbstractValidator<DeleteReminderCommand>
    {
        public DeleteReminderCommandValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
