using Microsoft.EntityFrameworkCore;
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
        Assert.Fail();
    }

    [Test]
    public async Task DeleteTodo_ShouldReturnNullAndNotFoundError_IfTodoExistsButUserDoesNotOwnIt()
    {
        Assert.Fail();
    }

    [Test]
    public async Task DeleteTodo_ShouldSucceedAndDeleteTodo()
    {
        Assert.Fail();
    }

    [TearDown]
    public void TearDown() 
    { 
    
    }
}
