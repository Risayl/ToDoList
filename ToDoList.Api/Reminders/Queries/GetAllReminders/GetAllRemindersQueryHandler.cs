using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Reminders.Queries
{
    public class GetAllRemindersQueryHandler : IRequestHandler<GetAllRemindersQuery, IEnumerable<Reminder>>
    {
        private readonly IRepository<Reminder> _reminderRepository;

        public GetAllRemindersQueryHandler(IRepository<Reminder> reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }
        public async Task<IEnumerable<Reminder>> Handle(GetAllRemindersQuery request, CancellationToken cancellationToken)
        {
            var notes = await _reminderRepository.GetAllAsync();
            return notes;
        }
    }
}
