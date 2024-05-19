using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class GetTodosUnitTests
{
    private DataDbContext _dataDbContext;
    private TodoDataAccess _todoDataAccess;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DataDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        _dataDbContext = new DataDbContext(options);
        _todoDataAccess = new TodoDataAccess(_dataDbContext);
    }

    [Test]
    public async Task GetTodos_ShouldSucceedAndReturnNoTodos_IfUserHasNone()
    {
        Assert.Fail();
    }

    [Test]
    public async Task GetTodos_ShouldSucceedAndReturnTodos()
    {
        Assert.Fail();
    }

}
