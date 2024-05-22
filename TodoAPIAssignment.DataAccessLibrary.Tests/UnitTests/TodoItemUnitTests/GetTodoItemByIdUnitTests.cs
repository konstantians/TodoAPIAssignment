using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;
using TodoAPIAssignment.DataAccessLibrary.Models;
using FluentAssertions;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using Microsoft.Azure.Cosmos;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests.TodoItemUnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]

public class GetTodoItemByIdUnitTests
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

        CreateTodoItemResult createTodoItemResult = await _todoItemDataAccess.CreateUserTodoItemAsync("1", _testTodo!.Id!, new TodoItem() { 
            Title = "MyTodoItem", Description = "Todo Item Description", IsDone = false}
        );
        _testTodoItem = createTodoItemResult.TodoItem!;
    }

    [Test]
    public async Task GetTodoItemById_ShouldReturnNullAndNotFoundTodoMessage_IfTodoNotFound()
    {
        //Arrange
        string userId = "1";
        string bogusTodoId = "bogusTodoId";
        string todoItemId = _testTodoItem.Id!;

        //Act
        GetTodoItemResult result = await _todoItemDataAccess.GetUserTodoItemAsync(userId, bogusTodoId, todoItemId);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.TodoNotFound);
        result.TodoItem.Should().Be(null);
    }

    [Test]
    public async Task GetTodoItemById_ShouldReturnNullAndNotFoundTodoMessage_IfTodoExistsButUserDoesNotOwnIt()
    {

        //Arrange
        string bogusUserId = "bogusUserId";
        string todoId = _testTodo.Id!;
        string todoItemId = _testTodoItem.Id!;

        //Act
        GetTodoItemResult result = await _todoItemDataAccess.GetUserTodoItemAsync(bogusUserId, todoId, todoItemId);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.TodoNotFound);
        result.TodoItem.Should().Be(null);
    }

    [Test]
    public async Task GetTodoItemById_ShouldReturnNullAndNotFoundTodoItemMessage_IfTodoExistsButTodoItemDoesNotExist()
    {

        //Arrange
        string userId = "1";
        string todoId = _testTodo.Id!;
        string bogusTodoItemId = "bogusTodoItemId";

        //Act
        GetTodoItemResult result = await _todoItemDataAccess.GetUserTodoItemAsync(userId, todoId, bogusTodoItemId);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.TodoItemNotFound);
        result.TodoItem.Should().Be(null);
    }

    [Test]
    public async Task GetTodoItemById_ShouldReturnTodoItem()
    {

        //Arrange
        string userId = "1";
        string todoId = _testTodo.Id!;
        string todoItemId = _testTodoItem.Id!;

        //Act
        GetTodoItemResult result = await _todoItemDataAccess.GetUserTodoItemAsync(userId, todoId, todoItemId);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.None);
        result.TodoItem.Should().NotBeNull();
        result.TodoItem!.Id.Should().Be(todoItemId);
    }

    [TearDown]
    public void TearDown()
    {

    }
}
