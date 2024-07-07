using ToDoList.Models;
using ToDoList.Reminders.Queries;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;

public class GetReminderByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Reminder>> _reminderRepositoryMock;
    private readonly GetReminderByIdQueryHandler _handler;

    public GetReminderByIdQueryHandlerTests()
    {
        _reminderRepositoryMock = new Mock<IRepository<Reminder>>();
        _handler = new GetReminderByIdQueryHandler(_reminderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Reminder_By_Id()
    {
        // Arrange
        var query = new GetReminderByIdQuery
        {
            Id = 1,
        };
        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(query.Id))
                           .ReturnsAsync(new Reminder { Id = 1, Title = "Reminder 1", Text = "Text 1", ReminderTime = DateTime.UtcNow });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(query.Id);
        _reminderRepositoryMock.Verify(n => n.GetByIdAsync(query.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
    {
        // Arrange
        var query = new GetReminderByIdQuery
        {
            Id = 1,
        };
        _reminderRepositoryMock.Setup(n => n.GetByIdAsync(query.Id))
                           .ReturnsAsync((Reminder)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        exception.Message.Should().Be($"Reminder with ID {query.Id} not found.");
        _reminderRepositoryMock.Verify(n => n.GetByIdAsync(query.Id), Times.Once);
    }
}
