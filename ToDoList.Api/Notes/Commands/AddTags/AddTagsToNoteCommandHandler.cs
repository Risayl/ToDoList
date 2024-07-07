using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;
using ToDoList.Services;

namespace ToDoList.Notes.Commands
{
    public class AddTagsToNoteCommandHandler : IRequestHandler<AddTagsToNoteCommand, Note>
    {
        private readonly IRepository<Note> _noteRepository;
        private readonly IValidator<AddTagsToNoteCommand> _validator;
        private readonly ITagService _tagService;
        public AddTagsToNoteCommandHandler(IRepository<Note> noteRepository, IValidator<AddTagsToNoteCommand> validator, ITagService tagService)
        {
            _noteRepository = noteRepository;
            _validator = validator;
            _tagService = tagService;
        }

        public async Task<Note> Handle(AddTagsToNoteCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Note note = await _noteRepository.GetByIdAsync(request.Id);
            if (note == null)
            {
                throw new Exception($"Note with ID {request.Id} not found.");
            }
            note = await _tagService.AddTagsToNoteAsync(note, request.Tags);
            return note;
        }
    }
}
