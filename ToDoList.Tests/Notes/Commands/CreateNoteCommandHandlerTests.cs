using FluentValidation;
using ToDoList.Models;
using ToDoList.Notes.Commands;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class CreateNoteCommandHandlerTests
{
    private readonly Mock<IRepository<Note>> _noteRepositoryMock;
    private readonly CreateNoteCommandValidator _validator;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly CreateNoteCommandHandler _handler;

    public CreateNoteCommandHandlerTests()
    {
        _noteRepositoryMock = new Mock<IRepository<Note>>();
        _validator = new CreateNoteCommandValidator(_noteRepositoryMock.Object);
        _tagRepositoryMock = new Mock<ITagRepository>();
        _handler = new CreateNoteCommandHandler(_noteRepositoryMock.Object, _tagRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_Should_Add_Note_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateNoteCommand
        {
            Title = "Test Title",
            Text = "Test Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };
        var tags = new List<Tag>()
        {
            new Tag { Name = command.Tags[0] },
            new Tag { Name = command.Tags[1] }
        };
        _tagRepositoryMock.Setup(t => t.GetByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync((Tag)null);
        _noteRepositoryMock.Setup(n => n.AddAsync(It.IsAny<Note>()))
                           .ReturnsAsync(new Note { Title = command.Title, Text = command.Text, Tags = tags });

        // Act
        var validationResult =await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Text.Should().Be(command.Text);
        result.Tags.Should().Contain(tag => tag.Name == command.Tags[0]);
        result.Tags.Should().Contain(tag => tag.Name == command.Tags[1]);
        _noteRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Note>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Title()
    {
        // Arrange
        var command = new CreateNoteCommand
        {
            Text = "Test Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };
        
        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _noteRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Note>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Title_Already_Exists()
    {
        // Arrange
        var command = new CreateNoteCommand
        {
            Title = "Existing title",
            Text = "Test Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        var existingNote = new Note { Title = "Existing title", Text = "Some text" };
        _noteRepositoryMock.Setup(n => n.GetAllAsync()).ReturnsAsync(new List<Note> { existingNote });

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title already exists.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _noteRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Note>()), Times.Never);
    }
}
