using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests.TodoItemUnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class CreateTodoItemUnitTests
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
    public async Task CreateTodoItem_ShouldReturnNullAndTodoNotFoundError_IfTodoNotFound()
    {
        Assert.Fail();
    }

    [Test]
    public async Task CreateTodoItem_ShouldReturnNullAndTodoNotFoundError_IfTodoExistsButUserDoesNotOwnTodo()
    {
        Assert.Fail();
    }

    [Test]
    public async Task CreateTodoItem_ShouldSucceedAndCreateTodoItem()
    {
        Assert.Fail();
    }

    [TearDown]
    public void TearDown() 
    { 
    
    }
}
