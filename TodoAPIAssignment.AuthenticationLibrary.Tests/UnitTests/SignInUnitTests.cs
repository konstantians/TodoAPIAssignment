using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace TodoAPIAssignment.AuthenticationLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class SignInUnitTests
{
    private IConfiguration _config;
    private AuthDbContext _authDbContext;
    private AuthenticationDataAccess _authenticationDataAccess;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        _config = Substitute.For<IConfiguration>();
        _authDbContext = new AuthDbContext(options);
        _authenticationDataAccess = new AuthenticationDataAccess(_authDbContext, _config);
    }

    [Test]
    public async Task LogIn_ShouldFailAndReturnErrorMessage_IfConfigurationNotSet()
    {
        Assert.Fail();
    }

    [Test]
    public async Task LogIn_ShouldFailAndReturnErrorMessage_IfInvalidCredentials()
    {
        Assert.Fail();
    }

    [Test]
    public async Task SignIn_ShouldSucceedAndReturnToken()
    {
        Assert.Fail();
    }

    [TearDown]
    public void TearDown()
    {

    }
}
