using FluentValidation;
using ToDoList.Models;
using ToDoList.Tags.Commands;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class DeleteTagCommandHandlerTests
{
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock;
    private readonly DeleteTagCommandValidator _validator;
    private readonly DeleteTagCommandHandler _handler;

    public DeleteTagCommandHandlerTests()
    {
        _tagRepositoryMock = new Mock<IRepository<Tag>>();
        _validator = new DeleteTagCommandValidator();
        _handler = new DeleteTagCommandHandler(_tagRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_Should_Delete_Tag_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeleteTagCommand
        {
            Id = 1
        };

        _tagRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new Tag { Id = 1, Name = "Name"});

        _tagRepositoryMock.Setup(n => n.DeleteAsync(It.IsAny<int?>()))
                           .Returns(Task.CompletedTask)
                           .Callback<int?>(id =>
                           {
                               _tagRepositoryMock.Setup(n => n.GetByIdAsync(id))
                                                  .ReturnsAsync((Tag)null);
                           });

        // Act
        var validationResult = await _validator.TestValidateAsync(command);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        _tagRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Once);
        var deletedTag = await _tagRepositoryMock.Object.GetByIdAsync(command.Id);
        deletedTag.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Id()
    {
        // Arrange
        var command = new DeleteTagCommand
        {
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _tagRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var command = new DeleteTagCommand
        {
            Id = 2
        };

        _tagRepositoryMock.Setup(n => n.GetByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((Tag)null);

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be($"Tag with ID {command.Id} not found.");
        _tagRepositoryMock.Verify(n => n.DeleteAsync(It.Is<int>(id => id == command.Id)), Times.Never);
    }
}
