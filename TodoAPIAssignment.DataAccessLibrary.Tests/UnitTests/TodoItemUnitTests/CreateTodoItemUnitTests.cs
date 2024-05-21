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
public class CreateTodoItemUnitTests
{
    private TodoDataAccess _todoDataAccess;
    private DataDbContext _dataDbContext;
    private TodoItemDataAccess _todoItemDataAccess;
    private Todo _testTodo;

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<DataDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;


        _dataDbContext = new DataDbContext(options);
        _todoDataAccess = new TodoDataAccess(_dataDbContext);
        _todoItemDataAccess = new TodoItemDataAccess(_dataDbContext, _todoDataAccess);
        CreateTodoResult result = await _todoDataAccess.CreateTodoAsync(new Todo() { Title = "MyTodo", IsDone = false, UserId = "1" });
        _testTodo = result.Todo!;
    }

    [Test]
    public async Task CreateTodoItem_ShouldReturnNullAndTodoNotFoundError_IfTodoNotFound()
    {
        //Arrange
        string userId = "1";
        string bogusTodoId = "bogusId";
        TodoItem todoItem = new TodoItem()
        {
            Title = "Todo Item Title",
            Description = "Todo Item Description",
            IsDone = false
        };

        //Act
        CreateTodoItemResult createTodoItemResult = await _todoItemDataAccess.CreateUserTodoItemAsync(userId, bogusTodoId, todoItem);

        //Assert
        createTodoItemResult.Should().NotBeNull();
        createTodoItemResult.ErrorCode.Should().Be(ErrorCode.TodoNotFound);
        createTodoItemResult.TodoItem.Should().BeNull();
    }

    [Test]
    public async Task CreateTodoItem_ShouldReturnNullAndTodoNotFoundError_IfTodoExistsButUserDoesNotOwnTodo()
    {
        //Arrange
        string bogusUserId = "bogusUserId";
        string todoId = _testTodo.Id!;
        TodoItem todoItem = new TodoItem()
        {
            Title = "Todo Item Title",
            Description = "Todo Item Description",
            IsDone = false
        };

        //Act
        CreateTodoItemResult createTodoItemResult = await _todoItemDataAccess.CreateUserTodoItemAsync(bogusUserId, todoId, todoItem);

        //Assert
        createTodoItemResult.Should().NotBeNull();
        createTodoItemResult.ErrorCode.Should().Be(ErrorCode.TodoNotFound);
        createTodoItemResult.TodoItem.Should().BeNull();
    }

    [Test]
    public async Task CreateTodoItem_ShouldSucceedAndCreateTodoItem()
    {
        //Arrange
        string userId = "1";
        string bogusTodoId = _testTodo.Id!;
        string todoItemTitle = "Todo Item Title";
        TodoItem todoItem = new TodoItem()
        {
            Title = todoItemTitle,
            Description = "Todo Item Description",
            IsDone = false
        };

        //Act
        CreateTodoItemResult createTodoItemResult = await _todoItemDataAccess.CreateUserTodoItemAsync(userId, bogusTodoId, todoItem);

        //Assert
        createTodoItemResult.Should().NotBeNull();
        createTodoItemResult.ErrorCode.Should().Be(ErrorCode.None);
        createTodoItemResult.TodoItem!.Title.Should().Be(todoItemTitle);
    }

    [TearDown]
    public void TearDown() 
    { 
    
    }
}
