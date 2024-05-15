using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace TodoAPIAssignment.AuthenticationLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class LogInUnitTests
{
    private IConfiguration _config;
    private AuthDbContext _authDbContext;
    private AuthenticationDataAccess _authenticationDataAccess;

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
        await _authenticationDataAccess.SignUpAsync("konstantinos", "password", "kinnaskonstantinos0@gmail.com")!;
    }

    [Test]
    public async Task LogIn_ShouldFailAndReturnErrorMessage_IfConfigurationNotSet()
    {
        //Arrange
        //missing configuration causes the test to fail. This is 1 way to get to the exception
        _config["Jwt:Key"].ReturnsNull();
        _config["Jwt:Issuer"].ReturnsNull();
        _config["Jwt:Audience"].ReturnsNull();

        //Act
        string? result = await _authenticationDataAccess.SignInAsync("konstantinos", "password")!;

        //Assert
        result.Should().NotBeNull();
        result.Should().Be("DatabaseError");
    }

    [Test]
    public async Task LogIn_ShouldFailAndReturnErrorMessage_IfInvalidCredentials()
    {
        //Arrange

        //Act
        string? result = await _authenticationDataAccess.SignInAsync("konstantinos", "falsePassword")!;

        //Assert
        result.Should().NotBeNull();
        result.Should().Be("InvalidCredentials");
    }

    [Test]
    public async Task LogIn_ShouldSucceedAndReturnToken()
    {
        //Arrange

        //Act
        string? result = await _authenticationDataAccess.SignInAsync("konstantinos", "password")!;

        //Assert
        result.Should().NotBeNull();
        result.Should().NotBe("DatabaseError");
        result.Should().NotBe("InvalidCredentials");
        result.Length.Should().BeGreaterThan(30);    
    }

    [TearDown]
    public void TearDown()
    {

    }
}
