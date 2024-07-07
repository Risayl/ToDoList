using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Notes.Commands;
using ToDoList.Notes.Queries;


namespace ToDoList.Controllers;

[ApiController]
[Route("api/v1/function/note")]

public class NoteController : ControllerBase
{
    private readonly IMediator _mediator;
    public NoteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAllNotes([FromQuery] GetAllNotesQuery query)
    {
        var notes = await _mediator.Send(query);
        return Ok(notes);
    }

    [HttpGet("get-one")]
    public async Task<IActionResult> GetNoteById([FromQuery] GetNoteByIdQuery query)
    {
        var note = await _mediator.Send(query);
        return Ok(note);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody]CreateNoteCommand command)
    {
        var note = await _mediator.Send(command);
        if (note == null)
        {
            return BadRequest("Failed to create note.");
        }
        return Ok(note);
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateNoteCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Note updated successfully" });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteNoteCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Reminder deleted successfully" });
    }

    [HttpPost("addtags")]
    public async Task<IActionResult> AddTags([FromBody] AddTagsToNoteCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Tags added successfully" });
    }

    [HttpPost("removetags")]
    public async Task<IActionResult> RemoveTags([FromBody] RemoveTagsFromNoteCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Tag removed successfully" });
    }
}

