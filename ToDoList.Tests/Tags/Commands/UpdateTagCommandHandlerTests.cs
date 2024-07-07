using FluentValidation;
using ToDoList.Models;
using ToDoList.Tags.Commands;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class UpdateTagCommandHandlerTests
{
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock;
    private readonly UpdateTagCommandValidator _validator;
    private readonly Mock<ITagRepository> _tagIRepositoryMock;
    private readonly UpdateTagCommandHandler _handler;

    public UpdateTagCommandHandlerTests()
    {
        _tagRepositoryMock = new Mock<IRepository<Tag>>();
        _tagIRepositoryMock = new Mock<ITagRepository>();
        _validator = new UpdateTagCommandValidator(_tagIRepositoryMock.Object);
        _handler = new UpdateTagCommandHandler(_tagRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_Should_Update_Tag_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateTagCommand
        {
            Id = 1,
            Name = "New Name",
        };

        _tagRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new Tag { Id = 1, Name = "Old Name"});
        _tagIRepositoryMock.Setup(t => t.GetByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync((Tag)null);
        _tagRepositoryMock.Setup(n => n.UpdateAsync(It.IsAny<Tag>()));

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        _tagRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Tag>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new UpdateTagCommand
        {
            Name = "New Name",
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Tag>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Name()
    {
        // Arrange
        var command = new UpdateTagCommand
        {
            Id = 1
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Tag name is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Tag>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Name_Already_Exists()
    {
        // Arrange
        var command = new UpdateTagCommand
        {
            Id = 1,
            Name = "Existing name"
        };

        var existingTag = new Tag { Name = "Existing name" };
        _tagIRepositoryMock.Setup(t => t.GetByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync(existingTag);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Tag already exists.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Tag>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var command = new UpdateTagCommand
        {
            Id = 1,
            Name = "Existing name"
        };

        _tagRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Tag)null);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Tag with ID {command.Id} not found.");
        _tagRepositoryMock.Verify(n => n.UpdateAsync(It.IsAny<Tag>()), Times.Never);
    }
}
