using FluentValidation;
using ToDoList.Models;
using ToDoList.Notes.Commands;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class UpdateNoteCommandHandlerTests
{
    private readonly Mock<IRepository<Note>> _noteRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly UpdateNoteCommandValidator _validator;
    private readonly UpdateNoteCommandHandler _handler;

    public UpdateNoteCommandHandlerTests()
    {
        _noteRepositoryMock = new Mock<IRepository<Note>>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _validator = new UpdateNoteCommandValidator(_noteRepositoryMock.Object);
        _handler = new UpdateNoteCommandHandler(_noteRepositoryMock.Object, _tagRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_Should_Update_Note_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateNoteCommand
        {
            Id = 1,
            Title = "New Title",
            Text = "New Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        _noteRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new Note { Id = 1, Title = "Old Title", Text = "Old Text" });
        _tagRepositoryMock.Setup(t => t.GetByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync((Tag)null);
        _noteRepositoryMock.Setup(n => n.UpdateAsync(It.IsAny<Note>()));

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Text.Should().Be(command.Text);
        result.Tags.Should().Contain(tag => tag.Name == command.Tags[0]);
        result.Tags.Should().Contain(tag => tag.Name == command.Tags[1]);
        _noteRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Note>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new UpdateNoteCommand
        {
            Title = "New Title",
            Text = "New Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _noteRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Note>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Title()
    {
        // Arrange
        var command = new UpdateNoteCommand
        {
            Id = 1,
            Text = "New Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _noteRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Note>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Title_Already_Exists()
    {
        // Arrange
        var command = new UpdateNoteCommand
        {
            Id = 1,
            Title = "Existing title",
            Text = "New Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        var existingNote = new Note { Title = "Existing title", Text = "Some text" };
        _noteRepositoryMock.Setup(n => n.GetAllAsync()).ReturnsAsync(new List<Note> { existingNote });

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title already exists.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _noteRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Note>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var command = new UpdateNoteCommand
        {
            Id = 2,
            Title = "New Title",
            Text = "New Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        _noteRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Note)null);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Note with ID {command.Id} not found.");
        _noteRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Note>()), Times.Never);
    }
}
