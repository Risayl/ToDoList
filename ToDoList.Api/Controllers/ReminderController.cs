using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Notes.Commands;
using ToDoList.Reminders.Commands;
using ToDoList.Reminders.Queries;

namespace ToDoList.Controllers;

[ApiController]
[Route("api/v1/function/reminder")]

public class ReminderController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReminderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAllReminders([FromQuery] GetAllRemindersQuery query)
    {
        var reminders = await _mediator.Send(query);
        return Ok(reminders);
    }

    [HttpGet("get-one")]
    public async Task<IActionResult> GetReminderById([FromQuery] GetReminderByIdQuery query)
    {
        var reminder = await _mediator.Send(query);
        return Ok(reminder);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody]CreateReminderCommand command)
    {
        var reminder = await _mediator.Send(command);
        if (reminder == null)
        {
            return BadRequest("Failed to create reminder.");
        }
        return Ok(reminder);
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateReminderCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Reminder updated successfully" });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteReminderCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Reminder deleted successfully" });
    }

    [HttpPost("addtags")]
    public async Task<IActionResult> AddTags([FromBody] AddTagsToReminderCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Tags added successfully" });
    }

    [HttpPost("removetags")]
    public async Task<IActionResult> RemoveTags([FromBody] RemoveTagsFromReminderCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Tag removed successfully" });
    }
}

