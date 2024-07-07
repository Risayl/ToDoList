using FluentValidation;
using ToDoList.Models;
using ToDoList.Tags.Commands;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class CreateTagCommandHandlerTests
{
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock;
    private readonly CreateTagCommandValidator _validator;
    private readonly Mock<ITagRepository> _tagIRepositoryMock;
    private readonly CreateTagCommandHandler _handler;

    public CreateTagCommandHandlerTests()
    {
        _tagRepositoryMock = new Mock<IRepository<Tag>>();
        _tagIRepositoryMock = new Mock<ITagRepository>();
        _validator = new CreateTagCommandValidator(_tagIRepositoryMock.Object);
        _handler = new CreateTagCommandHandler(_tagRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_Should_Add_Tag_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateTagCommand
        {
            Name = "Test"
        };

        _tagIRepositoryMock.Setup(t => t.GetByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync((Tag)null);
        _tagRepositoryMock.Setup(n => n.AddAsync(It.IsAny<Tag>()))
                           .ReturnsAsync(new Tag { Name = command.Name});

        // Act
        var validationResult =await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        _tagRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Tag>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Name()
    {
        // Arrange
        var command = new CreateTagCommand
        {};

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Tag name is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Tag>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Name_Already_Exists()
    {
        // Arrange
        var command = new CreateTagCommand
        {
            Name = "Existing name"
        };

        var existingTag= new Tag { Name = "Existing name"};
        _tagIRepositoryMock.Setup(t => t.GetByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync(existingTag);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Tag already exists.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Tag>()), Times.Never);
    }
}
