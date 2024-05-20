using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class GetTodoByIdUnitTests
{
    private DataDbContext _dataDbContext;
    private TodoDataAccess _todoDataAccess;
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
    public async Task GetTodoById_ShouldReturnNullAndNotFoundMessage_IfTodoNotFound()
    {
        //Arrange
        string bogusTodoId = "bogusTodoId";
        string userId = "1";

        //Act
        GetTodoResult result = await _todoDataAccess.GetUserTodoAsync(userId, bogusTodoId);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.NotFound);
        result.Todo.Should().BeNull();
    }

    [Test]
    public async Task GetTodoById_ShouldReturnNullAndNotFoundMessage_IfTodoExistsButUserDoesNotOwnIt()
    {
        //Arrange
        string todoId = _testTodo!.Id!;
        string bogusUserId = "bogusUserId";

        //Act
        GetTodoResult result = await _todoDataAccess.GetUserTodoAsync(bogusUserId, todoId);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.NotFound);
        result.Todo.Should().BeNull();
    }

    [Test]
    public async Task GetTodoById_ShouldReturnTodo()
    {
        //Arrange
        string todoId = _testTodo!.Id!;
        string userId = "1";

        //Act
        GetTodoResult result = await _todoDataAccess.GetUserTodoAsync(userId, todoId);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.None);
        result.Todo.Should().NotBeNull();
        result!.Todo!.Id.Should().Be(todoId);
        result!.Todo!.UserId.Should().Be(userId);
    }

    [TearDown]
    public void TearDown()
    {

    }
}
