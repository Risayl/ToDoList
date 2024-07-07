using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Tags.Commands;
using ToDoList.Tags.Queries;

namespace ToDoList.Controllers;

[ApiController]
[Route("api/v1/function/tag")]

public class TagController : ControllerBase
{
    private readonly IMediator _mediator;
    public TagController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAllReminders([FromQuery] GetAllTagsQuery query)
    {
        var reminders = await _mediator.Send(query);
        return Ok(reminders);
    }

    [HttpGet("get-one")]
    public async Task<IActionResult> GetReminderById([FromQuery] GetTagByIdQuery query)
    {
        var reminder = await _mediator.Send(query);
        return Ok(reminder);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody]CreateTagCommand command)
    {
        var reminder = await _mediator.Send(command);
        if (reminder == null)
        {
            return BadRequest("Failed to create tag.");
        }
        return Ok(reminder);
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTagCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Tag updated successfully" });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteTagCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Tag deleted successfully" });
    }
}

