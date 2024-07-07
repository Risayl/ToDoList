using ToDoList.Models;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using ToDoList.Reminders.Queries;

public class GetAllRemindersQueryHandlerTests
{
    private readonly Mock<IRepository<Reminder>> _reminderRepositoryMock;
    private readonly GetAllRemindersQueryHandler _handler;

    public GetAllRemindersQueryHandlerTests()
    {
        _reminderRepositoryMock = new Mock<IRepository<Reminder>>();
        _handler = new GetAllRemindersQueryHandler(_reminderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Reminders()
    {
        // Arrange
        var query = new GetAllRemindersQuery{};
        var expectedReminders = new List<Reminder>
        {
            new Reminder { Id = 1, Title = "Reminder 1", Text = "Text 1", ReminderTime = DateTime.UtcNow },
            new Reminder { Id = 2, Title = "Reminder 2", Text = "Text 2", ReminderTime = DateTime.UtcNow }
        };

        _reminderRepositoryMock.Setup(n => n.GetAllAsync()).ReturnsAsync(expectedReminders);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedReminders);
        _reminderRepositoryMock.Verify(n => n.GetAllAsync(), Times.Once);
    }
}
