using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TodoAPIAssignement.API.Tests.IntegrationTests.HelperMethods;
using TodoAPIAssignment.API;
using TodoAPIAssignment.API.Models.RequestModels;

namespace TodoAPIAssignement.API.Tests.IntegrationTests.ControllerTests;

[TestFixture]
[Category("Integration")]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class AuthenticationControllerTests
{
    private HttpClient httpClient;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        httpClient = webApplicationFactory.CreateClient();
    }

    [SetUp]
    public void SetUp()
    {

    }

    [Test, Order(1)]
    public async Task SignUp_ShouldReturnOkAndAccessToken()
    {
        //Arrange
        SignUpRequestModel signUpRequestModel = new SignUpRequestModel()
        {
            Email = "kinnaskonstantinos0@gmail.com",
            Username = "konstantinos",
            Password = "password"
        };

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/authentication/signup", signUpRequestModel);
        string? tokenValue = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "token");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        tokenValue.Should().NotBeNullOrEmpty();
        tokenValue!.Length.Should().BeGreaterThan(30);
    }

    [Test, Order(2)]
    public async Task SignUp_ShouldFailAndReturnBadRequest_IfDuplicateUsername()
    {
        //Arrange
        SignUpRequestModel signUpRequestModel = new SignUpRequestModel()
        {
            Email = "differentEmail@gmail.com",
            Username = "konstantinos",
            Password = "password"
        };

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/authentication/signup", signUpRequestModel);
        string? errorMessageValue = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessageValue.Should().NotBeNullOrEmpty();
        errorMessageValue.Should().Be("DuplicateUsernameError");
    }

    [Test, Order(3)]
    public async Task SignUp_ShouldFailAndReturnBadRequest_IfDuplicateEmail()
    {
        //Arrange
        SignUpRequestModel signUpRequestModel = new SignUpRequestModel()
        {
            Email = "kinnaskonstantinos0@gmail.com",
            Username = "differentUsername",
            Password = "password"
        };

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/authentication/signup", signUpRequestModel);
        string? errorMessageValue = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessageValue.Should().NotBeNullOrEmpty();
        errorMessageValue.Should().Be("DuplicateEmailError");
    }

    [Test, Order(4)]
    public async Task LogIn_ShouldFailAndReturnBadRequest_IfInvalidCredentials()
    {
        //Arrange
        LogInRequestModel logInRequestModel = new LogInRequestModel()
        {
            Username = "bogusUser",
            Password = "bogusPassword"
        };

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/authentication/login", logInRequestModel);
        string? errorMessageValue = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessageValue.Should().NotBeNullOrEmpty();
        errorMessageValue.Should().Be("InvalidCredentialsError");
    }

    [Test, Order(5)]
    public async Task LogIn_ShouldReturnOkAndAccessToken()
    {
        //Arrange
        LogInRequestModel logInRequestModel = new LogInRequestModel()
        {
            Username = "konstantinos",
            Password = "password"
        };

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/authentication/login", logInRequestModel);
        string? tokenValue = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "token");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        tokenValue.Should().NotBeNullOrEmpty();
        tokenValue!.Length.Should().BeGreaterThan(30);
    }

    [TearDown]
    public void TearDown()
    {

    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        httpClient.Dispose();
        await ResetDatabaseHelperMethods.ResetNoSqlEmailDatabase();
    }
}
