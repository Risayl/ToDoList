using FluentValidation;
using MediatR;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Notes.Commands
{
    public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Note>
    {
        private readonly IRepository<Note> _noteRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IValidator<CreateNoteCommand> _validator;
        public CreateNoteCommandHandler(IRepository<Note> noteRepository, ITagRepository tagRepository, IValidator<CreateNoteCommand> validator)
        {
            _noteRepository = noteRepository;
            _tagRepository = tagRepository;
            _validator = validator;
        }

        public async Task<Note> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
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
            Note note = await _noteRepository.AddAsync(new Note { Title = request.Title, Text = request.Text, Tags = tags});
            return note;
        }
    }
}
