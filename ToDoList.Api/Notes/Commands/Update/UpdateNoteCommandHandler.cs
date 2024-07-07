using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Notes.Commands
{
    public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Note>
    {
        private readonly IRepository<Note> _noteRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IValidator<UpdateNoteCommand> _validator;
        public UpdateNoteCommandHandler(IRepository<Note> noteRepository, ITagRepository tagRepository, IValidator<UpdateNoteCommand> validator)
        {
            _noteRepository = noteRepository;
            _tagRepository = tagRepository;
            _validator = validator;
        }

        public async Task<Note> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            Note note = await _noteRepository.GetByIdAsync(request.Id);
            if (note == null)
            {
                throw new Exception($"Note with ID {request.Id} not found.");
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
            note.Title = request.Title;
            note.Text = request.Text;
            note.Tags = tags;
            await _noteRepository.UpdateAsync(note);
            return note;
        }
    }
}
