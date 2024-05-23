using Microsoft.EntityFrameworkCore;
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
        Assert.Fail();
    }

    [Test]
    public async Task DeleteTodoItem_ShouldReturnNullAndNotFoundTodoMessage_IfTodoExistsButUserDoesNotOwnIt()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DeleteTodoItem_ShouldReturnNullAndNotFoundTodoItemMessage_IfTodoExistsButTodoItemDoesNotExist()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DeleteTodoItem_ShouldDeleteTodoItem()
    {
        Assert.Fail();
    }
}
