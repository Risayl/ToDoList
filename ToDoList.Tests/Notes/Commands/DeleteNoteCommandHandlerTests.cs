using FluentValidation;
using ToDoList.Models;
using ToDoList.Notes.Commands;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class DeleteNoteCommandHandlerTests
{
    private readonly Mock<IRepository<Note>> _noteRepositoryMock;
    private readonly DeleteNoteCommandValidator _validator;
    private readonly DeleteNoteCommandHandler _handler;

    public DeleteNoteCommandHandlerTests()
    {
        _noteRepositoryMock = new Mock<IRepository<Note>>();
        _validator = new DeleteNoteCommandValidator();
        _handler = new DeleteNoteCommandHandler(_noteRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_Should_Delete_Note_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeleteNoteCommand
        {
            Id = 1
        };

        _noteRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new Note { Id = 1, Title = "Title", Text = "Text" });

        _noteRepositoryMock.Setup(n => n.DeleteAsync(It.IsAny<int?>()))
                           .Returns(Task.CompletedTask)
                           .Callback<int?>(id =>
                           {
                               _noteRepositoryMock.Setup(n => n.GetByIdAsync(id))
                                                  .ReturnsAsync((Note)null);
                           });

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        _noteRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Once);
        var deletedNote = await _noteRepositoryMock.Object.GetByIdAsync(command.Id);
        deletedNote.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new DeleteNoteCommand
        {
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _noteRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var command = new DeleteNoteCommand
        {
            Id = 2
        };

        _noteRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Note)null);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Note with ID {command.Id} not found.");
        _noteRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Never);
    }
}
