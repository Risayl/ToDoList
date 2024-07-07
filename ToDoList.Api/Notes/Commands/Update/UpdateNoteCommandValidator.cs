using FluentValidation;
using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Notes.Commands
{
    public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
    {
        private readonly IRepository<Note> _noteRepository;
        public UpdateNoteCommandValidator(IRepository<Note> noteRepository) 
        {
            _noteRepository = noteRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MustAsync(BeUniqueTitle).WithMessage("Title already exists."); 
        }
        private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
        {
            var allNotes = await _noteRepository.GetAllAsync();
            var existingNote = allNotes.FirstOrDefault(n => n.Title == title);
            
            if(existingNote != null) 
            {
                return false;
            }
            return true;
        }
    }
}
