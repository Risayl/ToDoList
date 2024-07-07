using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Notes.Queries
{
    public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Note>
    {
        private readonly IRepository<Note> _noteRepository;

        public GetNoteByIdQueryHandler(IRepository<Note> noteRepository)
        {
            _noteRepository = noteRepository;
        }
        public async Task<Note> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            Note note = await _noteRepository.GetByIdAsync(request.Id);
            if (note == null)
            {
                throw new Exception($"Note with ID {request.Id} not found.");
            }
            return note;
        }
    }
}
