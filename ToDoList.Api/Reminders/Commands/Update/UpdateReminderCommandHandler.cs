using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Reminders.Commands
{
    public class UpdateReminderCommandHandler : IRequestHandler<UpdateReminderCommand, Reminder>
    {
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IValidator<UpdateReminderCommand> _validator;
        public UpdateReminderCommandHandler(IRepository<Reminder> reminderRepository, ITagRepository tagRepository, IValidator<UpdateReminderCommand> validator)
        {
            _reminderRepository = reminderRepository;
            _tagRepository = tagRepository;
            _validator = validator;
        }

        public async Task<Reminder> Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Reminder reminder = await _reminderRepository.GetByIdAsync(request.Id);
            if (reminder == null)
            {
                throw new Exception($"Reminder with ID {request.Id} not found.");
            }
            var tags = new List<Tag>();
            if (request.Tags != null)
            {
                foreach (var tagName in request.Tags)
                {
                    var tag = await _tagRepository.GetByNameAsync(tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        await _tagRepository.AddAsync(tag);
                    }
                    tags.Add(tag);
                }
            }
            reminder.Title = request.Title;
            reminder.Text = request.Text;
            reminder.ReminderTime = (DateTime.Parse(request.ReminderTime)).ToUniversalTime();
            reminder.Tags = tags;
            await _reminderRepository.UpdateAsync(reminder);
            return reminder;
        }
    }
}
