using FluentValidation;
using ToDoList.Models;
using ToDoList.Notes.Commands;
using ToDoList.Repositories;
using ToDoList.Services;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class RemoveTagsFromNoteCommandHandlerTests
{
    private readonly Mock<IRepository<Note>> _noteRepositoryMock;
    private readonly Mock<ITagService> _tagServiceMock;
    private readonly RemoveTagsFromNoteCommandValidator _validator;
    private readonly RemoveTagsFromNoteCommandHandler _handler;

    public RemoveTagsFromNoteCommandHandlerTests()
    {
        _noteRepositoryMock = new Mock<IRepository<Note>>();
        _tagServiceMock = new Mock<ITagService>();
        _validator = new RemoveTagsFromNoteCommandValidator();
        _handler = new RemoveTagsFromNoteCommandHandler(_noteRepositoryMock.Object, _validator, _tagServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Remove_Tags_From_Note_When_Command_Is_Valid()
    {
        // Arrange
        var command = new RemoveTagsFromNoteCommand
        {
            Id = 1,
            TagId = 1
        };
        var existingNote = new Note
        {
            Id = 1,
            Title = "Test Note",
            Text = "Test Text",
            Tags = new List<Tag>() { new Tag { Name = "Tag1" }, new Tag { Name = "Tag2" } }
        };
        var updatedNote = new Note
        {
            Id = 1,
            Title = "Title",
            Text = "Text",
            Tags = new List<Tag> { new Tag { Name = "Tag2" } }
        };
        _noteRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(existingNote);
        _tagServiceMock.Setup(n => n.RemoveTagsFromNoteAsync(It.IsAny<Note>(), It.IsAny<int>()))
                            .ReturnsAsync(updatedNote);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        result.Should().NotBeNull();;
        result.Tags.Should().NotContain(tag => tag.Id == command.TagId);
        result.Tags.Should().Contain(tag => tag.Name == "Tag2");
        _tagServiceMock.Verify(n => n.RemoveTagsFromNoteAsync(It.IsAny<Note>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new RemoveTagsFromNoteCommand
        {
            TagId = 1
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagServiceMock.Verify(n => n.RemoveTagsFromNoteAsync(It.IsAny<Note>(), command.TagId), Times.Never); 
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_TagId()
    {
        // Arrange
        var command = new RemoveTagsFromNoteCommand
        {
            Id = 1
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert

        validationResult.ShouldHaveValidationErrorFor(x => x.TagId).WithErrorMessage("Tag Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagServiceMock.Verify(n => n.RemoveTagsFromNoteAsync(It.IsAny<Note>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        var command = new RemoveTagsFromNoteCommand
        {
            Id = 1,
            TagId = 1
        };
        _noteRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Note)null);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Note with ID {command.Id} not found.");
        _tagServiceMock.Verify(n => n.RemoveTagsFromNoteAsync(It.IsAny<Note>(), command.TagId), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_TagId_Not_Found()
    {
        var command = new RemoveTagsFromNoteCommand
        {
            Id = 1,
            TagId = 5
        };
        var existingNote = new Note
        {
            Id = 2,
            Title = "Test Note",
            Text = "Test Text",
            Tags = new List<Tag>() { new Tag { Name = "Tag1" }, new Tag { Name = "Tag2" } }
        };
        _noteRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(existingNote);
        _tagServiceMock.Setup(n => n.RemoveTagsFromNoteAsync(It.IsAny<Note>(), It.IsAny<int>()))
                            .ThrowsAsync(new Exception($"Tag with ID {command.TagId} not found in note."));

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Tag with ID {command.TagId} not found in note.");
        _tagServiceMock.Verify(n => n.RemoveTagsFromNoteAsync(It.IsAny<Note>(), command.TagId), Times.Once);
    }
}
