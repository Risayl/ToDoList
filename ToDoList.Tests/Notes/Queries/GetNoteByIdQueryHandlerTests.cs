using ToDoList.Models;
using ToDoList.Notes.Queries;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;

public class GetNoteByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Note>> _noteRepositoryMock;
    private readonly GetNoteByIdQueryHandler _handler;

    public GetNoteByIdQueryHandlerTests()
    {
        _noteRepositoryMock = new Mock<IRepository<Note>>();
        _handler = new GetNoteByIdQueryHandler(_noteRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Note_By_Id()
    {
        // Arrange
        var query = new GetNoteByIdQuery
        {
            Id = 1,
        };
        _noteRepositoryMock.Setup(n => n.GetByIdAsync(query.Id))
                           .ReturnsAsync(new Note { Id = 1, Title = "Note 1", Text = "Text 1" });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(query.Id);
        _noteRepositoryMock.Verify(n => n.GetByIdAsync(query.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var query = new GetNoteByIdQuery
        {
            Id = 1,
        };
        _noteRepositoryMock.Setup(n => n.GetByIdAsync(query.Id))
                           .ReturnsAsync((Note)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        exception.Message.Should().Be($"Note with ID {query.Id} not found.");
        _noteRepositoryMock.Verify(n => n.GetByIdAsync(query.Id), Times.Once);
    }
}
