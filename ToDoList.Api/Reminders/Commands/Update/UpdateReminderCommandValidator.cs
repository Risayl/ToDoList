using FluentValidation;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Reminders.Commands
{
    public class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
    {
        private readonly IRepository<Reminder> _reminderRepository;
        public UpdateReminderCommandValidator(IRepository<Reminder> reminderRepository) 
        {
            _reminderRepository = reminderRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MustAsync(BeUniqueTitle).WithMessage("Title already exists.");

            RuleFor(x => x.ReminderTime)
           .NotEmpty().WithMessage("ReminderTime is required.")
           .Must(BeAValidDate).WithMessage("ReminderTime must be a valid date.")
           .Must(BeInTheFuture).WithMessage("ReminderTime must be in the future.");
        }
        private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
        {
            var allNotes = await _reminderRepository.GetAllAsync();
            var existingNote = allNotes.FirstOrDefault(n => n.Title == title);
            
            if(existingNote != null) 
            {
                return false;
            }
            return true;
        }
        private bool BeAValidDate(string date)
        {
            return DateTime.TryParse(date, out _);
        }

        private bool BeInTheFuture(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                return parsedDate > DateTime.Now;
            }
            return false;
        }
    }
}
