using Microsoft.EntityFrameworkCore;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class UpdateTodoUnitTests
{
    private TodoDataAccess _todoDataAccess;
    private DataDbContext _dataDbContext;

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
    public async Task UpdateTodo_ShouldReturnNull_IfTodoNotFound()
    {
        Assert.Fail();
    }

    [Test]
    public async Task UpdateTodo_ShouldReturnNull_IfTodoExistsButUserDoesNotOwnIt()
    {
        Assert.Fail();
    }

    [Test]
    public async Task UpdateTodo_ShouldSucceedAndUpdateTodo()
    {
        Assert.Fail();
    }

    [TearDown]
    public void TearDown() 
    { 
    
    }
}
