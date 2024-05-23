using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests.TodoItemUnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class DeleteTodoItemUnitTests
{
    private DataDbContext _dataDbContext;
    private TodoDataAccess _todoDataAccess;
    private TodoItemDataAccess _todoItemDataAccess;
    private Todo _testTodo;
    private TodoItem _testTodoItem;

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

        _todoItemDataAccess = new TodoItemDataAccess(_dataDbContext, _todoDataAccess);

        CreateTodoItemResult createTodoItemResult = await _todoItemDataAccess.CreateUserTodoItemAsync("1", _testTodo!.Id!, new TodoItem()
        {
            Title = "MyTodoItem",
            Description = "Todo Item Description",
            IsDone = false
        });

        _testTodoItem = createTodoItemResult.TodoItem!;
    }

    [Test]
    public async Task DeleteTodoItem_ShouldReturnNullAndNotFoundTodoMessage_IfTodoNotFound()
    {
        //Arrange
        string bogusUserId = "1";
        string bogusTodoId = "bogusTodoId";
        string todoItemId = _testTodoItem.Id!;

        //Act
        ErrorCode errorCode = await _todoItemDataAccess.DeleteUserTodoItemAsync(bogusUserId, bogusTodoId, todoItemId);

        //Assert
        errorCode.Should().Be(ErrorCode.TodoNotFound);
    }

    [Test]
    public async Task DeleteTodoItem_ShouldReturnNullAndNotFoundTodoMessage_IfTodoExistsButUserDoesNotOwnIt()
    {
        //Arrange
        string bogusUserId = "bogusUserId";
        string todoId = _testTodo.Id!;
        string todoItemId = _testTodoItem.Id!;

        //Act
        ErrorCode errorCode = await _todoItemDataAccess.DeleteUserTodoItemAsync(bogusUserId, todoId, todoItemId);

        //Assert
        errorCode.Should().Be(ErrorCode.TodoNotFound);
    }

    [Test]
    public async Task DeleteTodoItem_ShouldReturnNullAndNotFoundTodoItemMessage_IfTodoExistsButTodoItemDoesNotExist()
    {
        //Arrange
        string userId = "1";
        string todoId = _testTodo.Id!;
        string bogusTodoItemId = "bogusTodoItemId";

        //Act
        ErrorCode errorCode = await _todoItemDataAccess.DeleteUserTodoItemAsync(userId, todoId, bogusTodoItemId);

        //Assert
        errorCode.Should().Be(ErrorCode.TodoItemNotFound);
    }

    [Test]
    public async Task DeleteTodoItem_ShouldDeleteTodoItem()
    {
        //Arrange
        string userId = "1";
        string todoId = _testTodo.Id!;
        string todoItemId = _testTodoItem.Id!;

        //Act
        ErrorCode errorCode = await _todoItemDataAccess.DeleteUserTodoItemAsync(userId, todoId, todoItemId);
        GetTodoItemResult getTodoItemResultAfterDelete = await _todoItemDataAccess.GetUserTodoItemAsync(userId, todoId, todoItemId);

        //Assert
        errorCode.Should().Be(ErrorCode.None);
        getTodoItemResultAfterDelete.Should().NotBeNull();
        getTodoItemResultAfterDelete.ErrorCode.Should().Be(ErrorCode.TodoItemNotFound);
        getTodoItemResultAfterDelete.TodoItem.Should().BeNull();
    }
}
