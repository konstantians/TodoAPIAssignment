using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests.TodoUnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class UpdateTodoUnitTests
{
    private TodoDataAccess _todoDataAccess;
    private DataDbContext _dataDbContext;
    private Todo _testTodo;

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<DataDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        _dataDbContext = new DataDbContext(options);
        _todoDataAccess = new TodoDataAccess(_dataDbContext);
        CreateTodoResult result = await _todoDataAccess.CreateTodoAsync(new Todo() { Title = "MyTodo", IsDone = false, UserId = "1" });
        _testTodo = result.Todo!;
    }

    [Test]
    public async Task UpdateTodo_ShouldReturnNullAndNotFoundError_IfTodoNotFound()
    {
        //Arrange
        Todo updatedTodo = new Todo()
        {
            Id = "bogusTodoId",
            Title = "updatedTitle",
            IsDone = true,
            UserId = "1"
        };

        //Act
        UpdateTodoResult result = await _todoDataAccess.UpdateUserTodoAsync(updatedTodo);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.TodoNotFound);
        result.Todo.Should().BeNull();
    }

    [Test]
    public async Task UpdateTodo_ShouldReturnNullAndNotFoundError_IfTodoExistsButUserDoesNotOwnIt()
    {
        //Arrange
        Todo updatedTodo = new Todo()
        {
            Id = _testTodo.Id,
            Title = "updatedTitle",
            IsDone = true,
            UserId = "bogusUserId"
        };

        //Act
        UpdateTodoResult result = await _todoDataAccess.UpdateUserTodoAsync(updatedTodo);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.TodoNotFound);
        result.Todo.Should().BeNull();
    }

    [Test]
    public async Task UpdateTodo_ShouldSucceedAndUpdateTodo()
    {
        //Arrange
        string updatedTitle = "updatedTitle";
        Todo updatedTodo = new Todo()
        {
            Id = _testTodo.Id,
            Title = updatedTitle,
            IsDone = true,
            UserId = _testTodo.UserId
        };

        //Act
        UpdateTodoResult result = await _todoDataAccess.UpdateUserTodoAsync(updatedTodo);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.None);
        result.Todo.Should().NotBeNull();
        result.Todo!.UserId.Should().Be(_testTodo.UserId);
        result.Todo!.Title.Should().Be(updatedTitle);
    }

    [TearDown]
    public void TearDown()
    {

    }
}
