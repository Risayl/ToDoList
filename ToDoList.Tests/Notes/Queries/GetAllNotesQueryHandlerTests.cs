using ToDoList.Models;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using ToDoList.Notes.Queries;

public class GetAllNotesQueryHandlerTests
{
    private readonly Mock<IRepository<Note>> _noteRepositoryMock;
    private readonly GetAllNotesQueryHandler _handler;

    public GetAllNotesQueryHandlerTests()
    {
        _noteRepositoryMock = new Mock<IRepository<Note>>();
        _handler = new GetAllNotesQueryHandler(_noteRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Notes()
    {
        // Arrange
        var query = new GetAllNotesQuery{};
        var expectedNotes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Text = "Text 1" },
            new Note { Id = 2, Title = "Note 2", Text = "Text 2" }
        };

        _noteRepositoryMock.Setup(n => n.GetAllAsync()).ReturnsAsync(expectedNotes);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedNotes);
        _noteRepositoryMock.Verify(n => n.GetAllAsync(), Times.Once);
    }
}
