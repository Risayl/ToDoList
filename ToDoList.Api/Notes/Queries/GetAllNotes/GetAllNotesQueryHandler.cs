using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Notes.Queries
{
    public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, IEnumerable<Note>>
    {
        private readonly IRepository<Note> _noteRepository;

        public GetAllNotesQueryHandler(IRepository<Note> noteRepository)
        {
            _noteRepository = noteRepository;
        }
        public async Task<IEnumerable<Note>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            var notes = await _noteRepository.GetAllAsync();
            return notes;
        }
    }
}
