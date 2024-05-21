using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests.TodoUnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class CreateTodoUnitTests
{
    private DataDbContext _dataDbContext;
    private TodoDataAccess _todoDataAccess;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DataDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        _dataDbContext = new DataDbContext(options);
        _todoDataAccess = new TodoDataAccess(_dataDbContext);
    }

    [Test]
    public async Task CreateTodo_ShouldSucceedAndCreateTodo()
    {
        //Arrange
        Todo todo = new Todo
        {
            Title = "MyTodo",
            UserId = "1",
            IsDone = false
        };

        //Act
        CreateTodoResult createTodoResult = await _todoDataAccess.CreateTodoAsync(todo);

        //Assert
        createTodoResult.Should().NotBeNull();
        createTodoResult.ErrorCode.Should().Be(ErrorCode.None);
        createTodoResult.Todo!.Id.Should().NotBeNullOrEmpty();
        createTodoResult.Todo!.CreatedAt.Should().NotBeNull();
    }

    [TearDown]
    public void TearDown()
    {

    }
}
