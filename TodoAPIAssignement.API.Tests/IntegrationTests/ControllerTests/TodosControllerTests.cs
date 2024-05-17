using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TodoAPIAssignement.API.Tests.IntegrationTests.HelperMethods;
using TodoAPIAssignment.API;
using TodoAPIAssignment.API.Models.AuthenticationControllerModels.RequestModels;
using TodoAPIAssignment.API.Models.TodoControllerModels.RequestModels;
using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignement.API.Tests.IntegrationTests.ControllerTests;

[TestFixture]
[Category("Integration")]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
internal class TodosControllerTests
{
    private HttpClient httpClient;
    private string? _accessToken;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        httpClient = webApplicationFactory.CreateClient();

        SignUpRequestModel signUpRequestModel = new SignUpRequestModel()
        {
            Email = "kinnaskonstantinos0@gmail.com",
            Username = "differentUsername",
            Password = "password"
        };
        
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/authentication/signup", signUpRequestModel);
        _accessToken = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "token");
    }

    [SetUp]
    public void SetUp()
    {

    }

    [Test, Order(1)]
    public async Task CreateTodo_ShouldReturnInvalidTokenError_IfTokenNotValid()
    {
        //Arrange
        CreateTodoRequestModel createTodoRequestModel = new CreateTodoRequestModel()
        {
            Title = "MyTodo",
            IsDone = false,
            Token = "BogusToken"
        };

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/todos/", createTodoRequestModel);
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("InvalidAccessToken");
    }

    [Test, Order(2)]
    public async Task CreateTodo_ShouldSucceedAndReturnTodoAndLocation()
    {
        //Arrange
        CreateTodoRequestModel createTodoRequestModel = new CreateTodoRequestModel()
        {
            Title = "MyTodo",
            IsDone = false,
            Token = _accessToken
        };

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/todos/", createTodoRequestModel);
        string? responseBody = await response.Content.ReadAsStringAsync();
        Todo? todo = JsonSerializer.Deserialize<Todo>(responseBody, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("MyTodo");
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
