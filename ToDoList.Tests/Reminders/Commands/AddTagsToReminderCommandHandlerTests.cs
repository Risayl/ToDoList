using FluentValidation;
using ToDoList.Models;
using ToDoList.Reminders.Commands;
using ToDoList.Repositories;
using ToDoList.Services;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class AddTagsToReminderCommandHandlerTests
{
    private readonly Mock<IRepository<Reminder>> _reminderRepositoryMock;
    private readonly Mock<ITagService> _tagServiceMock;
    private readonly AddTagsToReminderCommandValidator _validator;
    private readonly AddTagsToReminderCommandHandler _handler;

    public AddTagsToReminderCommandHandlerTests()
    {
        _reminderRepositoryMock = new Mock<IRepository<Reminder>>();
        _tagServiceMock = new Mock<ITagService>();
        _validator = new AddTagsToReminderCommandValidator();
        _handler = new AddTagsToReminderCommandHandler(_reminderRepositoryMock.Object, _validator, _tagServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Add_Tags_To_Reminder_When_Command_Is_Valid()
    {
        // Arrange
        var command = new AddTagsToReminderCommand
        {
            Id = 1,
            Tags = new List<string> { "Tag1", "Tag2" }
        };
        var updatedReminder = new Reminder
        {
            Id = 1,
            Title = "Title",
            Text = "Text",
            ReminderTime = DateTime.UtcNow,
            Tags = new List<Tag> { new Tag { Name = "Tag1" }, new Tag { Name = "Tag2" } }
        };
        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new Reminder { Id = 1, Title = "Title", Text = "Text", ReminderTime = DateTime.UtcNow });
        _tagServiceMock.Setup(n => n.AddTagsToReminderAsync(It.IsAny<Reminder>(), command.Tags))
                            .ReturnsAsync(updatedReminder);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        result.Should().NotBeNull();;
        result.Tags.Should().Contain(tag => tag.Name == command.Tags[0]);
        result.Tags.Should().Contain(tag => tag.Name == command.Tags[1]);
        _tagServiceMock.Verify(n => n.AddTagsToReminderAsync(It.IsAny<Reminder>(), command.Tags), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new AddTagsToReminderCommand
        {
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagServiceMock.Verify(n => n.AddTagsToReminderAsync(It.IsAny<Reminder>(), command.Tags), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Tags()
    {
        var command = new AddTagsToReminderCommand
        {
            Id = 1
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Tags).WithErrorMessage("Tags are required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagServiceMock.Verify(n => n.AddTagsToReminderAsync(It.IsAny<Reminder>(), command.Tags), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var command = new AddTagsToReminderCommand
        {
            Id = 1,
            Tags = new List<string> { "Tag1", "Tag2" }
        };
        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Reminder)null);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Reminder with ID {command.Id} not found.");
        _tagServiceMock.Verify(n => n.AddTagsToReminderAsync(It.IsAny<Reminder>(), command.Tags), Times.Never);
    }
}
