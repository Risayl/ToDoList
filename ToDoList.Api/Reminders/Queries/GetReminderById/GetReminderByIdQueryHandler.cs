using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Reminders.Queries
{
    public class GetReminderByIdQueryHandler : IRequestHandler<GetReminderByIdQuery, Reminder>
    {
        private readonly IRepository<Reminder> _reminderRepository;

        public GetReminderByIdQueryHandler(IRepository<Reminder> reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }
        public async Task<Reminder> Handle(GetReminderByIdQuery request, CancellationToken cancellationToken)
        {
            Reminder reminder = await _reminderRepository.GetByIdAsync(request.Id);
            if (reminder == null)
            {
                throw new Exception($"Reminder with ID {request.Id} not found.");
            }
            return reminder;
        }
    }
}
