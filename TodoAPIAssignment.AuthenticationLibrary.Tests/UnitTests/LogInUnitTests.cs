using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using TodoAPIAssignment.AuthenticationLibrary.Enums;
using TodoAPIAssignment.AuthenticationLibrary.Models;

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
        AuthenticationResult? result = await _authenticationDataAccess.LogInAsync("konstantinos", "password")!;

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.DatabaseError);
    }

    [Test]
    public async Task LogIn_ShouldFailAndReturnErrorMessage_IfInvalidCredentials()
    {
        //Arrange

        //Act
        AuthenticationResult? result = await _authenticationDataAccess.LogInAsync("konstantinos", "falsePassword")!;

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.InvalidCredentials);
    }

    [Test]
    public async Task LogIn_ShouldSucceedAndReturnToken()
    {
        //Arrange

        //Act
        AuthenticationResult? result = await _authenticationDataAccess.LogInAsync("konstantinos", "password")!;

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.None);
        result.Token!.Should().NotBeNull();
        result.Token!.Length.Should().BeGreaterThan(30);
    }

    [TearDown]
    public void TearDown()
    {

    }
}
