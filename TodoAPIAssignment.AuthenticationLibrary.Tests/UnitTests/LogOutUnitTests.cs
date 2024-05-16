using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace TodoAPIAssignment.AuthenticationLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class LogOutUnitTests
{
    private IConfiguration _config;
    private AuthDbContext _authDbContext;
    private AuthenticationDataAccess _authenticationDataAccess;
    private string _accessToken;

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        _config = Substitute.For<IConfiguration>();
        _authDbContext = new AuthDbContext(options);
        _authenticationDataAccess = new AuthenticationDataAccess(_authDbContext, _config);

        _config["Jwt:Key"].Returns("9hZP3lYnqBhGv2AsN5U8rxXKdJpW4Mmt");
        _config["Jwt:Issuer"].Returns("TodoAPIAssignment.API");
        _config["Jwt:Audience"].Returns("TodoAPIAssignment.API");
        var result = await _authenticationDataAccess.SignUpAsync("konstantinos", "password", "kinnaskonstantinos0@gmail.com")!;
        _accessToken = result.Token!;
    }

    [Test]
    public async Task LogOut_ShouldFailAndReturnErrorMessage_IfConfigurationNotSet()
    {
        Assert.Fail();
    }

    [Test]
    public async Task LogOut_ShouldFailAndReturnErrorMessage_IfInvalidAccessToken()
    {
        Assert.Fail();
    }

    [Test]
    public async Task LogOut_ShouldSucceedAndUpdateTokenCounter()
    {
        Assert.Fail();
    }

    [TearDown]
    public void TearDown() 
    {
        
    }
}
