using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class DeleteTodoUnitTests
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
    public async Task DeleteTodo_ShouldReturnNullAndNotFoundError_IfTodoNotFound()
    {
        //Arrange
        string bogusTodoId = "bogusTodoId";

        //Act
        ErrorCode result = await _todoDataAccess.DeleteUserTodoAsync("1", bogusTodoId);

        //Assert
        result.Should().Be(ErrorCode.NotFound);
    }

    [Test]
    public async Task DeleteTodo_ShouldReturnNullAndNotFoundError_IfTodoExistsButUserDoesNotOwnIt()
    {
        //Arrange
        string bogusUserId = "bogusUserId";

        //Act
        ErrorCode result = await _todoDataAccess.DeleteUserTodoAsync(bogusUserId, _testTodo!.Id!);

        //Assert
        result.Should().Be(ErrorCode.NotFound);
    }

    [Test]
    public async Task DeleteTodo_ShouldSucceedAndDeleteTodo()
    {

        //Arrange
        string userId = "1";
        string todoId = _testTodo!.Id!;

        //Act
        ErrorCode result = await _todoDataAccess.DeleteUserTodoAsync(userId, todoId);

        //Assert
        result.Should().Be(ErrorCode.None);
    }

    [TearDown]
    public void TearDown() 
    { 
    
    }
}
