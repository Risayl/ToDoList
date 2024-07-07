using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;
using ToDoList.Services;

namespace ToDoList.Reminders.Commands
{
    public class RemoveTagsFromReminderCommandHandler : IRequestHandler<RemoveTagsFromReminderCommand, Reminder>
    {
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly IValidator<RemoveTagsFromReminderCommand> _validator;
        private readonly ITagService _tagService;
        public RemoveTagsFromReminderCommandHandler(IRepository<Reminder> reminderRepository, IValidator<RemoveTagsFromReminderCommand> validator, ITagService tagService)
        {
            _reminderRepository = reminderRepository;
            _validator = validator;
            _tagService = tagService;
        }

        public async Task<Reminder> Handle(RemoveTagsFromReminderCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Reminder reminder = await _reminderRepository.GetByIdAsync(request.Id);
            if (reminder == null)
            {
                throw new Exception($"Reminder with ID {request.Id} not found.");
            }
            reminder = await _tagService.RemoveTagsFromReminderAsync(reminder, request.TagId);
            return reminder;
        }
    }
}
