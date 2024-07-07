using FluentValidation;
using ToDoList.Models;
using ToDoList.Reminders.Commands;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class DeleteReminderCommandHandlerTests
{
    private readonly Mock<IRepository<Reminder>> _reminderRepositoryMock;
    private readonly DeleteReminderCommandValidator _validator;
    private readonly DeleteReminderCommandHandler _handler;

    public DeleteReminderCommandHandlerTests()
    {
        _reminderRepositoryMock = new Mock<IRepository<Reminder>>();
        _validator = new DeleteReminderCommandValidator();
        _handler = new DeleteReminderCommandHandler(_reminderRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_Should_Delete_Reminder_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeleteReminderCommand
        {
            Id = 1
        };

        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new Reminder { Id = 1, Title = "Title", Text = "Text", ReminderTime = DateTime.UtcNow });

        _reminderRepositoryMock.Setup(n => n.DeleteAsync(It.IsAny<int?>()))
                           .Returns(Task.CompletedTask)
                           .Callback<int?>(id =>
                           {
                               _reminderRepositoryMock.Setup(n => n.GetByIdAsync(id))
                                                  .ReturnsAsync((Reminder)null);
                           });

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        _reminderRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Once);
        var deletedReminder = await _reminderRepositoryMock.Object.GetByIdAsync(command.Id);
        deletedReminder.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new DeleteReminderCommand
        {
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _reminderRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var command = new DeleteReminderCommand
        {
            Id = 2
        };

        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Reminder)null);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Reminder with ID {command.Id} not found.");
        _reminderRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Never);
    }
}
