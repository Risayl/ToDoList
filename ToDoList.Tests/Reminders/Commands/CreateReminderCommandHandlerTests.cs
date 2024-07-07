using FluentValidation;
using ToDoList.Models;
using ToDoList.Reminders.Commands;
using ToDoList.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation.TestHelper;

public class CreateReminderCommandHandlerTests
{
    private readonly Mock<IRepository<Reminder>> _reminderRepositoryMock;
    private readonly CreateReminderCommandValidator _validator;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly CreateReminderCommandHandler _handler;

    public CreateReminderCommandHandlerTests()
    {
        _reminderRepositoryMock = new Mock<IRepository<Reminder>>();
        _validator = new CreateReminderCommandValidator(_reminderRepositoryMock.Object);
        _tagRepositoryMock = new Mock<ITagRepository>();
        _handler = new CreateReminderCommandHandler(_reminderRepositoryMock.Object, _tagRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_Should_Add_Reminder_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateReminderCommand
        {
            Title = "Test Title",
            Text = "Test Text",
            ReminderTime = "2025-07-01T01:00:00",
            Tags = new List<string> { "Tag1", "Tag2" }
        };
        var tags = new List<Tag>()
        {
            new Tag { Name = command.Tags[0] },
            new Tag { Name = command.Tags[1] }
        };
        _tagRepositoryMock.Setup(t => t.GetByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync((Tag)null);
        _reminderRepositoryMock.Setup(n => n.AddAsync(It.IsAny<Reminder>()))
                           .ReturnsAsync(new Reminder { Title = command.Title, Text = command.Text, ReminderTime = (DateTime.Parse(command.ReminderTime)).ToUniversalTime(),  Tags = tags });

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
        _reminderRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Reminder>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_Title()
    {
        // Arrange
        var command = new CreateReminderCommand
        {
            Text = "Test Text",
            ReminderTime = "2025-07-01T01:00:00",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _reminderRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Reminder>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Title_Already_Exists()
    {
        // Arrange
        var command = new CreateReminderCommand
        {
            Title = "Existing Title",
            Text = "Test Text",
            ReminderTime = "2025-07-01T01:00:00",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        var existingReminder = new Reminder { Title = "Existing Title", Text = "Some text", ReminderTime = DateTime.Parse("2025-07-01T01:00:00Z") };
        _reminderRepositoryMock.Setup(n => n.GetAllAsync()).ReturnsAsync(new List<Reminder> { existingReminder });

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title already exists.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _reminderRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Reminder>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_ReminderTime()
    {
        // Arrange
        var command = new CreateReminderCommand
        {
            Title = "Existing Title",
            Text = "Test Text",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ReminderTime).WithErrorMessage("ReminderTime is required.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _reminderRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Reminder>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_ReminderTime_Is_Not_A_Valid_Date()
    {
        // Arrange
        var command = new CreateReminderCommand
        {
            Title = "Existing Title",
            Text = "Test Text",
            ReminderTime = "invalid-date",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ReminderTime).WithErrorMessage("ReminderTime must be a valid date.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _reminderRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Reminder>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_No_ReminderTime_Is_Not_In_Future()
    {
        // Arrange
        var command = new CreateReminderCommand
        {
            Title = "Existing Title",
            Text = "Test Text",
            ReminderTime = "2023-07-01T01:00:00",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(command);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ReminderTime).WithErrorMessage("ReminderTime must be in the future.");
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        _reminderRepositoryMock.Verify(n => n.AddAsync(It.IsAny<Reminder>()), Times.Never);
    }
}
