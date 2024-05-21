using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests.TodoUnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class GetTodosUnitTests
{
    private DataDbContext _dataDbContext;
    private TodoDataAccess _todoDataAccess;

    [SetUp]
    public void SeTup()
    {
        var options = new DbContextOptionsBuilder<DataDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        _dataDbContext = new DataDbContext(options);
        _todoDataAccess = new TodoDataAccess(_dataDbContext);
    }

    [Test]
    public async Task GetTodos_ShouldSucceedAndReturnNoTodos_IfUserHasNone()
    {
        //Arrange
        string userId = "1";

        //Act
        GetTodosResult getTodosResult = await _todoDataAccess.GetUserTodosAsync(userId);

        //Assert
        getTodosResult.Should().NotBeNull();
        getTodosResult.ErrorCode.Should().Be(ErrorCode.None);
        getTodosResult.Todos.Should().HaveCount(0);
    }

    [Test]
    public async Task GetTodos_ShouldSucceedAndReturnTodos()
    {
        //Arrange
        string userId = "1";
        Todo todo = new Todo
        {
            Title = "MyTodo",
            UserId = "1",
            IsDone = false
        };
        await _todoDataAccess.CreateTodoAsync(todo);

        //Act
        GetTodosResult getTodosResult = await _todoDataAccess.GetUserTodosAsync(userId);

        //Assert
        getTodosResult.Should().NotBeNull();
        getTodosResult.ErrorCode.Should().Be(ErrorCode.None);
        getTodosResult.Todos.Should().HaveCount(1);
    }

    [TearDown]
    public void TearDown()
    {

    }

}
