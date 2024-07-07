using FluentValidation;
using ToDoList.Models;
using ToDoList.Notes.Commands;
using ToDoList.Repositories;
using ToDoList.Services;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class AddTagsToNoteCommandHandlerTests
{
    private readonly Mock<IRepository<Note>> _noteRepositoryMock;
    private readonly Mock<ITagService> _tagServiceMock;
    private readonly AddTagsToNoteCommandValidator _validator;
    private readonly AddTagsToNoteCommandHandler _handler;

    public AddTagsToNoteCommandHandlerTests()
    {
        _noteRepositoryMock = new Mock<IRepository<Note>>();
        _tagServiceMock = new Mock<ITagService>();
        _validator = new AddTagsToNoteCommandValidator();
        _handler = new AddTagsToNoteCommandHandler(_noteRepositoryMock.Object, _validator, _tagServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Add_Tags_To_Note_When_Command_Is_Valid()
    {
        // Arrange
        var command = new AddTagsToNoteCommand
        {
            Id = 1,
            Tags = new List<string> { "Tag1", "Tag2" }
        };
        var updatedNote = new Note
        {
            Id = 1,
            Title = "Title",
            Text = "Text",
            Tags = new List<Tag> { new Tag { Name = "Tag1" }, new Tag { Name = "Tag2" } }
        };
        _noteRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new Note { Id = 1, Title = "Title", Text = "Text" });
        _tagServiceMock.Setup(n => n.AddTagsToNoteAsync(It.IsAny<Note>(), command.Tags))
                            .ReturnsAsync(updatedNote);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        result.Should().NotBeNull();;
        result.Tags.Should().Contain(tag => tag.Name == command.Tags[0]);
        result.Tags.Should().Contain(tag => tag.Name == command.Tags[1]);
        _tagServiceMock.Verify(n => n.AddTagsToNoteAsync(It.IsAny<Note>(), command.Tags), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new AddTagsToNoteCommand
        {
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagServiceMock.Verify(n => n.AddTagsToNoteAsync(It.IsAny<Note>(), command.Tags), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Tags()
    {
        var command = new AddTagsToNoteCommand
        {
            Id = 1
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Tags).WithErrorMessage("Tags are required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagServiceMock.Verify(n => n.AddTagsToNoteAsync(It.IsAny<Note>(), command.Tags), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var command = new AddTagsToNoteCommand
        {
            Id = 1,
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
        _tagServiceMock.Verify(n => n.AddTagsToNoteAsync(It.IsAny<Note>(), command.Tags), Times.Never);
    }
}
