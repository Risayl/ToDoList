using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Reminders.Commands
{
    public class DeleteReminderCommandHandler : IRequestHandler<DeleteReminderCommand, Reminder>
    {
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly IValidator<DeleteReminderCommand> _validator;
        public DeleteReminderCommandHandler(IRepository<Reminder> reminderRepository, IValidator<DeleteReminderCommand> validator)
        {
            _reminderRepository = reminderRepository;
            _validator = validator;
        }

        public async Task<Reminder> Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Reminder reminder = await _reminderRepository.GetByIdAsync(request.Id);
            if (reminder == null)
            {
                throw new Exception($"Reminder with ID {request.Id} not found.");
            }
            await _reminderRepository.DeleteAsync(request.Id);
            return reminder;
        }
    }
}
