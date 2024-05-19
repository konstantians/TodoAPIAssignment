using Microsoft.EntityFrameworkCore;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class GetTodoByIdUnitTests
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
    public async Task GetTodoById_ShouldReturnNull_IfTodoNotFound()
    {
        Assert.Fail();
    }

    [Test]
    public async Task GetTodoById_ShouldReturnNull_IfTodoExistsButUserDoesNotOwnIt()
    {
        Assert.Fail();
    }

    [Test]
    public async Task GetTodoById_ShouldReturnTodo()
    {
        Assert.Fail();
    }

    [TearDown]
    public void TearDown()
    {

    }
}
