using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests.TodoItemUnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class UpdateTodoItemUnitTests
{
    private TodoDataAccess _todoDataAccess;
    private DataDbContext _dataDbContext;
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
        _todoItemDataAccess = new TodoItemDataAccess(_dataDbContext, _todoDataAccess);
        CreateTodoResult result = await _todoDataAccess.CreateTodoAsync(new Todo() { Title = "MyTodo", IsDone = false, UserId = "1" });
        _testTodo = result.Todo!;

        CreateTodoItemResult createTodoItemResult = await _todoItemDataAccess.CreateUserTodoItemAsync("1", _testTodo!.Id!, new TodoItem() { 
            Title = "MyTodoItem", Description = "My Todo Item Description", IsDone = false}
        );
        _testTodoItem = createTodoItemResult.TodoItem!;
    }

    [Test]
    public async Task UpdateTodoItem_ShouldReturnNullAndNotFoundTodoMessage_IfTodoNotFound()
    {
        //Arrange
        string userId = "1";
        string bogusTodoId = "bogusTodoId";
        TodoItem updatedTodoItem = new TodoItem()
        {
            Id = _testTodoItem.Id!,
            Title = "TodoItemUpdatedTitle",
            Description = "Todo Item Updated Description",
            IsDone = true
        };

        //Act
        UpdateTodoItemResult result = await _todoItemDataAccess.UpdateUserTodoItemAsync(userId, bogusTodoId, updatedTodoItem);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.TodoNotFound);
        result.TodoItem.Should().Be(null);
    }

    [Test]
    public async Task UpdateTodoItem_ShouldReturnNullAndNotFoundTodoMessage_IfTodoExistsButUserDoesNotOwnIt()
    {
        //Arrange
        string bogusUserId = "bogusUserId";
        string todoId = _testTodo.Id!;
        TodoItem updatedTodoItem = new TodoItem()
        {
            Id = _testTodoItem.Id!,
            Title = "TodoItemUpdatedTitle",
            Description = "Todo Item Updated Description",
            IsDone = true
        };

        //Act
        UpdateTodoItemResult result = await _todoItemDataAccess.UpdateUserTodoItemAsync(bogusUserId, todoId, updatedTodoItem);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.TodoNotFound);
        result.TodoItem.Should().Be(null);

    }

    [Test]
    public async Task UpdateTodoItem_ShouldReturnNullAndNotFoundTodoItemMessage_IfTodoExistsButTodoItemDoesNotExist()
    {
        //Arrange
        string userId = "1";
        string todoId = _testTodo.Id!;
        TodoItem updatedTodoItem = new TodoItem()
        {
            Id = "bogusTodoItemId",
            Title = "TodoItemUpdatedTitle",
            Description = "Todo Item Updated Description",
            IsDone = true
        };

        //Act
        UpdateTodoItemResult result = await _todoItemDataAccess.UpdateUserTodoItemAsync(userId, todoId, updatedTodoItem);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.TodoItemNotFound);
        result.TodoItem.Should().Be(null);

    }

    [Test]
    public async Task UpdateTodoItem_ShouldUpdateTheTodoItem()
    {
        //Arrange
        string userId = "1";
        string todoId = _testTodo.Id!;
        TodoItem updatedTodoItem = new TodoItem()
        {
            Id = _testTodoItem.Id!,
            Title = "TodoItemUpdatedTitle",
            Description = "Todo Item Updated Description",
            IsDone = true
        };

        //Act
        UpdateTodoItemResult result = await _todoItemDataAccess.UpdateUserTodoItemAsync(userId, todoId, updatedTodoItem);

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.None);
        result.TodoItem.Should().NotBeNull();
        result.TodoItem!.Id.Should().Be(updatedTodoItem.Id);
        result.TodoItem!.Title.Should().Be(updatedTodoItem.Title);
    }

    [TearDown]
    public void TearDown()
    {

    }
}
