namespace TodoAPIAssignment.DataAccessLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class CreateTodoUnitTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task CreateTodo_ShouldFailAndReturnError_IfDatabaseIsDown()
    {
        Assert.Fail();
    }

    [Test]
    public async Task CreateTodo_ShouldSucceedAndCreateTodo()
    {
        Assert.Fail();
    }

    [TearDown]
    public void Teardown() 
    { 
    
    }
}
