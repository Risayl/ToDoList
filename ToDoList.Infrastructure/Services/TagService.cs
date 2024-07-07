using ToDoList.Models;
using ToDoList.Repositories;

namespace ToDoList.Services
{
    public interface ITagService
    {
        Task<Note> AddTagsToNoteAsync(Note note, List<string> Tags);
        Task<Note> RemoveTagsFromNoteAsync(Note note, int tagId);
        Task<Reminder> AddTagsToReminderAsync(Reminder reminder, List<string> Tags);
        Task<Reminder> RemoveTagsFromReminderAsync(Reminder reminder, int tagId);
    }

    public class TagService : ITagService
    {
        private readonly IRepository<Note> _noteRepository;
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly ITagRepository _tagRepository;

        public TagService(IRepository<Note> noteRepository, IRepository<Reminder> reminderRepository, ITagRepository tagRepository)
        {
            _noteRepository = noteRepository;
            _reminderRepository = reminderRepository;
            _tagRepository = tagRepository;
        }
        public async Task<Note> AddTagsToNoteAsync(Note note, List<string> Tags)
        {
            var tags = new List<Tag>();
            if (Tags != null)
            {
                foreach (var tagName in Tags)
                {
                    var tag = await _tagRepository.GetByNameAsync(tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        await _tagRepository.AddAsync(tag);
                    }
                    note.Tags.Add(tag);
                }
            }
            await _noteRepository.UpdateAsync(note);
            return note;
        }
        public async Task<Note> RemoveTagsFromNoteAsync(Note note, int TagId)
        {
            Tag tag = await _tagRepository.GetByIdAsync(TagId);
            if (tag != null && note.Tags.Contains(tag))
            {
                note.Tags.Remove(tag);
                await _noteRepository.UpdateAsync(note);
            }
            else
            {
                throw new Exception($"Tag with ID {TagId} not found in note.");
            }
            return note;
        }
        public async Task<Reminder> AddTagsToReminderAsync(Reminder reminder, List<string> Tags)
        {
            var tags = new List<Tag>();
            if (Tags != null)
            {
                foreach (var tagName in Tags)
                {
                    var tag = await _tagRepository.GetByNameAsync(tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        await _tagRepository.AddAsync(tag);
                    }
                    reminder.Tags.Add(tag);
                }
            }
            await _reminderRepository.UpdateAsync(reminder);
            return reminder;
        }

        public async Task<Reminder> RemoveTagsFromReminderAsync(Reminder reminder, int TagId)
        {
            Tag tag = await _tagRepository.GetByIdAsync(TagId);
            if (tag != null && reminder.Tags.Contains(tag))
            {
                reminder.Tags.Remove(tag);
                await _reminderRepository.UpdateAsync(reminder);
            }
            else
            {
                throw new Exception($"Tag with ID {TagId} not found in reminder.");
            }
            return reminder;
        }
    }
}

