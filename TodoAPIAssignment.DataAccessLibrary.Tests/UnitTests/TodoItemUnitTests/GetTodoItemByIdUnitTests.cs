using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;
using TodoAPIAssignment.DataAccessLibrary.Models;
using FluentAssertions;
using TodoAPIAssignment.DataAccessLibrary.Enums;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests.TodoItemUnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]

public class GetTodoItemByIdUnitTests
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
    public async Task GetTodoItemById_ShouldReturnNullAndNotFoundTodoMessage_IfTodoNotFound()
    {
        Assert.Fail();
    }

    [Test]
    public async Task GetTodoItemById_ShouldReturnNullAndNotFoundTodoMessage_IfTodoExistsButUserDoesNotOwnIt()
    {
        Assert.Fail();
    }

    [Test]
    public async Task GetTodoItemById_ShouldReturnNullAndNotFoundTodoItemMessage_IfTodoExistsButTodoItemDoesNotExist()
    {
        Assert.Fail();
    }

    [Test]
    public async Task GetTodoItemById_ShouldReturnTodoItem()
    {
        Assert.Fail();
    }

    [TearDown]
    public void TearDown()
    {

    }
}
