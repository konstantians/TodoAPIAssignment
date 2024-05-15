using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using TodoAPIAssignment.AuthenticationLibrary.Enums;
using TodoAPIAssignment.AuthenticationLibrary.Models;

namespace TodoAPIAssignment.AuthenticationLibrary.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class SignUpUnitTests
{
    private AuthDbContext _authDbContext;
    private IConfiguration _config;
    private AuthenticationDataAccess _authenticationDataAccess;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        _authDbContext = new AuthDbContext(options);
        _config = Substitute.For<IConfiguration>();
        _authenticationDataAccess = new AuthenticationDataAccess(_authDbContext, _config);
    }

    [Test]
    public async Task SignUp_ShouldFailAndReturnErrorMessage_IfConfigurationNotSet()
    {
        //Arrange
        //missing configuration causes the test to fail. This is 1 way to get to the exception

        //Act
        AuthenticationResult? result = await _authenticationDataAccess.SignUpAsync("konstantinos", "password", "kinnaskonstantinos0@gmail.com")!;

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.DatabaseError);
    }

    [Test]
    public async Task SignUp_ShouldFailAndReturnErrorMessage_IfDuplicateUsername()
    {
        //Arrange
        _authDbContext.Users.Add(new AppUser() { Id = Guid.NewGuid().ToString(), Username = "konstantinos", 
            Email = "kinnaskonstantinos0@gmail.com", Password = "password"});
        await _authDbContext.SaveChangesAsync();

        //Act
        AuthenticationResult? result = await _authenticationDataAccess.SignUpAsync("konstantinos", "password", "giannis@gmail.com")!;

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.DuplicateUsername);
    }

    [Test]
    public async Task SignUp_ShouldFailAndReturnErrorMessage_IfDuplicateEmail()
    {
        //Arrange
        _authDbContext.Users.Add(new AppUser(){Id = Guid.NewGuid().ToString(), Username = "giannis",
            Email = "kinnaskonstantinos0@gmail.com", Password = "password"});
        await _authDbContext.SaveChangesAsync();

        //Act
        AuthenticationResult? result = await _authenticationDataAccess.SignUpAsync("konstantinos", "password", "kinnaskonstantinos0@gmail.com")!;

        //Assert
        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(ErrorCode.DuplicateEmail);
    }

    [Test]
    public async Task SignUp_ShouldSucceedAndReturnToken()
    {
        //Arrange
        _config["Jwt:Key"].Returns("9hZP3lYnqBhGv2AsN5U8rxXKdJpW4Mmt");
        _config["Jwt:Issuer"].Returns("TodoAPIAssignment.API");
        _config["Jwt:Audience"].Returns("TodoAPIAssignment.API");

        //Act
        AuthenticationResult? result = await _authenticationDataAccess.SignUpAsync("konstantinos", "password", "kinnaskonstantinos0@gmail.com")!;

        //Assert
        result.Should().NotBeNull();
        _authDbContext.Users.Should().HaveCount(1);
        result.ErrorCode.Should().Be(ErrorCode.None);
        result.Token.Should().NotBeNull();
        result.Token!.Length.Should().BeGreaterThan(30);
    }

    [TearDown]
    public void TearDown()
    {

    }
}