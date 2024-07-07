using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;
using ToDoList.Services;

namespace ToDoList.Notes.Commands
{
    public class RemoveTagsFromNoteCommandHandler : IRequestHandler<RemoveTagsFromNoteCommand, Note>
    {
        private readonly IRepository<Note> _noteRepository;
        private readonly IValidator<RemoveTagsFromNoteCommand> _validator;
        private readonly ITagService _tagService;
        public RemoveTagsFromNoteCommandHandler(IRepository<Note> noteRepository, IValidator<RemoveTagsFromNoteCommand> validator, ITagService tagService)
        {
            _noteRepository = noteRepository;
            _validator = validator;
            _tagService = tagService;
        }

        public async Task<Note> Handle(RemoveTagsFromNoteCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Note note = await _noteRepository.GetByIdAsync(request.Id);
            if (note == null)
            {
                throw new Exception($"Note with ID {request.Id} not found.");
            }
            note = await _tagService.RemoveTagsFromNoteAsync(note, request.TagId);
            return note;
        }
    }
}
