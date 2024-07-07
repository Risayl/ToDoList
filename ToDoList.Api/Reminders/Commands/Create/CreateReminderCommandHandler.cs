using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Reminders.Commands
{
    public class CreateReminderCommandHandler : IRequestHandler<CreateReminderCommand, Reminder>
    {
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IValidator<CreateReminderCommand> _validator;
        public CreateReminderCommandHandler(IRepository<Reminder> reminderRepository, ITagRepository tagRepository, IValidator<CreateReminderCommand> validator)
        {
            _reminderRepository = reminderRepository;
            _tagRepository = tagRepository;
            _validator = validator;
        }

        public async Task<Reminder> Handle(CreateReminderCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
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
            Reminder reminder = await _reminderRepository.AddAsync(new Reminder { Title = request.Title, Text = request.Text, ReminderTime = (DateTime.Parse(request.ReminderTime)).ToUniversalTime(), Tags = tags});
            return reminder;
        }
    }
}
