using ToDoList.Models;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using ToDoList.Tags.Queries;

public class GetAllTagsQueryHandlerTests
{
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock;
    private readonly GetAllTagsQueryHandler _handler;

    public GetAllTagsQueryHandlerTests()
    {
        _tagRepositoryMock = new Mock<IRepository<Tag>>();
        _handler = new GetAllTagsQueryHandler(_tagRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Tags()
    {
        // Arrange
        var query = new GetAllTagsQuery{};
        var expectedTags = new List<Tag>
        {
            new Tag { Id = 1, Name = "Tag 1"},
            new Tag { Id = 2, Name = "Tag 2"}
        };

        _tagRepositoryMock.Setup(n => n.GetAllAsync()).ReturnsAsync(expectedTags);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedTags);
        _tagRepositoryMock.Verify(n => n.GetAllAsync(), Times.Once);
    }
}
