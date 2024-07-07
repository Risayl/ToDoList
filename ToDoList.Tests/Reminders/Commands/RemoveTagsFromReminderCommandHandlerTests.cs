using FluentValidation;
using ToDoList.Models;
using ToDoList.Reminders.Commands;
using ToDoList.Repositories;
using ToDoList.Services;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class RemoveTagsFromReminderCommandHandlerTests
{
    private readonly Mock<IRepository<Reminder>> _reminderRepositoryMock;
    private readonly Mock<ITagService> _tagServiceMock;
    private readonly RemoveTagsFromReminderCommandValidator _validator;
    private readonly RemoveTagsFromReminderCommandHandler _handler;

    public RemoveTagsFromReminderCommandHandlerTests()
    {
        _reminderRepositoryMock = new Mock<IRepository<Reminder>>();
        _tagServiceMock = new Mock<ITagService>();
        _validator = new RemoveTagsFromReminderCommandValidator();
        _handler = new RemoveTagsFromReminderCommandHandler(_reminderRepositoryMock.Object, _validator, _tagServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Remove_Tags_From_Reminder_When_Command_Is_Valid()
    {
        // Arrange
        var command = new RemoveTagsFromReminderCommand
        {
            Id = 1,
            TagId = 1
        };
        var existingReminder = new Reminder
        {
            Id = 1,
            Title = "Test Note",
            Text = "Test Text",
            ReminderTime = DateTime.UtcNow,
            Tags = new List<Tag>() { new Tag { Name = "Tag1" }, new Tag { Name = "Tag2" } }
        };
        var updatedReminder = new Reminder
        {
            Id = 1,
            Title = "Title",
            Text = "Text",
            ReminderTime = DateTime.UtcNow,
            Tags = new List<Tag> { new Tag { Name = "Tag2" } }
        };
        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(existingReminder);
        _tagServiceMock.Setup(n => n.RemoveTagsFromReminderAsync(It.IsAny<Reminder>(), It.IsAny<int>()))
                            .ReturnsAsync(updatedReminder);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        result.Should().NotBeNull();;
        result.Tags.Should().NotContain(tag => tag.Id == command.TagId);
        result.Tags.Should().Contain(tag => tag.Name == "Tag2");
        _tagServiceMock.Verify(n => n.RemoveTagsFromReminderAsync(It.IsAny<Reminder>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new RemoveTagsFromReminderCommand
        {
            TagId = 1
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagServiceMock.Verify(n => n.RemoveTagsFromReminderAsync(It.IsAny<Reminder>(), command.TagId), Times.Never); 
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_TagId()
    {
        // Arrange
        var command = new RemoveTagsFromReminderCommand
        {
            Id = 1
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert

        validationResult.ShouldHaveValidationErrorFor(x => x.TagId).WithErrorMessage("Tag Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagServiceMock.Verify(n => n.RemoveTagsFromReminderAsync(It.IsAny<Reminder>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        var command = new RemoveTagsFromReminderCommand
        {
            Id = 1,
            TagId = 1
        };
        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Reminder)null);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Reminder with ID {command.Id} not found.");
        _tagServiceMock.Verify(n => n.RemoveTagsFromReminderAsync(It.IsAny<Reminder>(), command.TagId), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_TagId_Not_Found()
    {
        var command = new RemoveTagsFromReminderCommand
        {
            Id = 1,
            TagId = 5
        };
        var existingReminder = new Reminder
        {
            Id = 2,
            Title = "Test Note",
            Text = "Test Text",
            ReminderTime = DateTime.Now,
            Tags = new List<Tag>() { new Tag { Name = "Tag1" }, new Tag { Name = "Tag2" } }
        };
        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(existingReminder);
        _tagServiceMock.Setup(n => n.RemoveTagsFromReminderAsync(It.IsAny<Reminder>(), It.IsAny<int>()))
                            .ThrowsAsync(new Exception($"Tag with ID {command.TagId} not found in reminder."));

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Tag with ID {command.TagId} not found in reminder.");
        _tagServiceMock.Verify(n => n.RemoveTagsFromReminderAsync(It.IsAny<Reminder>(), command.TagId), Times.Once);
    }
}
