using ToDoList.Models;
using ToDoList.Tags.Queries;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;

public class GetTagByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock;
    private readonly GetTagByIdQueryHandler _handler;

    public GetTagByIdQueryHandlerTests()
    {
        _tagRepositoryMock = new Mock<IRepository<Tag>>();
        _handler = new GetTagByIdQueryHandler(_tagRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Tag_By_Id()
    {
        // Arrange
        var query = new GetTagByIdQuery
        {
            Id = 1,
        };
        _tagRepositoryMock.Setup(n => n.GetByIdAsync(query.Id))
                           .ReturnsAsync(new Tag { Id = 1, Name = "Tag 1"});

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(query.Id);
        _tagRepositoryMock.Verify(n => n.GetByIdAsync(query.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var query = new GetTagByIdQuery
        {
            Id = 1,
        };
        _tagRepositoryMock.Setup(n => n.GetByIdAsync(query.Id))
                           .ReturnsAsync((Tag)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        exception.Message.Should().Be($"Tag with ID {query.Id} not found.");
        _tagRepositoryMock.Verify(n => n.GetByIdAsync(query.Id), Times.Once);
    }
}
