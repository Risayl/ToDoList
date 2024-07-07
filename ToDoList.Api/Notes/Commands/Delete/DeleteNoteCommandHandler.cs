using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Notes.Commands
{
    public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, Note>
    {
        private readonly IRepository<Note> _noteRepository;
        private readonly IValidator<DeleteNoteCommand> _validator;
        public DeleteNoteCommandHandler(IRepository<Note> noteRepository, IValidator<DeleteNoteCommand> validator)
        {
            _noteRepository = noteRepository;
            _validator = validator;
        }

        public async Task<Note> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Note note = await _noteRepository.GetByIdAsync(request.Id);
            if (note == null)
            {
                throw new Exception($"Note with ID {request.Id} not found.");
            }
            await _noteRepository.DeleteAsync(request.Id);
            return note;
        }
    }
}
