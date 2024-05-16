using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using TodoAPIAssignment.AuthenticationLibrary.Enums;

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
        //Arrange
        _config["Jwt:Key"].ReturnsNull();
        _config["Jwt:Issuer"].ReturnsNull();
        _config["Jwt:Audience"].ReturnsNull();

        //Act
        ErrorCode result = await _authenticationDataAccess.LogOutAsync(_accessToken);

        //Assert
        result.Should().Be(ErrorCode.DatabaseError);
    }

    [Test]
    public async Task LogOut_ShouldFailAndReturnErrorMessage_IfInvalidAccessToken()
    {
        //Arrange

        //Act
        ErrorCode result = await _authenticationDataAccess.LogOutAsync("bogusToken");

        //Assert
        result.Should().Be(ErrorCode.InvalidAccessToken);
    }

    [Test]
    public async Task LogOut_ShouldFailAndReturnErrorMessage_IfSameTokenIsUsedTwice()
    {
        //Arrange
        await _authenticationDataAccess.LogOutAsync(_accessToken);

        //Act
        ErrorCode result = await _authenticationDataAccess.LogOutAsync(_accessToken);

        //Assert
        result.Should().Be(ErrorCode.InvalidAccessToken);
    }

    [Test]
    public async Task LogOut_ShouldSucceedAndUpdateTokenCounter()
    {
        //Arrange

        //Act
        ErrorCode result = await _authenticationDataAccess.LogOutAsync(_accessToken);

        //Assert
        result.Should().Be(ErrorCode.None);
    }
    

    [TearDown]
    public void TearDown() 
    {
        
    }
}
