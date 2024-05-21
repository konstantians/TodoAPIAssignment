using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TodoAPIAssignement.API.Tests.IntegrationTests.HelperMethods;
using TodoAPIAssignment.API;
using TodoAPIAssignment.API.Models.AuthenticationControllerModels.RequestModels;
using TodoAPIAssignment.API.Models.TodoControllerModels.RequestModels;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.API.Models.TodoItemsControllerModels.RequestModels;

namespace TodoAPIAssignement.API.Tests.IntegrationTests.ControllerTests;

[TestFixture]
[Category("Integration")]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class TodoItemsControllerTests
{
    private HttpClient httpClient;
    private string? _accessToken;
    private Todo? _testTodo;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        httpClient = webApplicationFactory.CreateClient();

        SignUpRequestModel signUpRequestModel = new SignUpRequestModel()
        {
            Email = "kinnaskonstantinos0@gmail.com",
            Username = "konstantinos",
            Password = "password"
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/authentication/signup", signUpRequestModel);
        _accessToken = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "token");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        CreateTodoRequestModel createTodoRequestModel = new CreateTodoRequestModel() { Title = "TodoItem", IsDone = false };
        
        HttpResponseMessage createTodoResponse = await httpClient.PostAsJsonAsync("api/todos", createTodoRequestModel);
        string? responseBody = await createTodoResponse.Content.ReadAsStringAsync();
        _testTodo = JsonSerializer.Deserialize<Todo>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    [Test, Order(1)]
    public async Task CreateTodoItem_ShouldReturnInvalidTokenError_IfTokenNotValid()
    {
        //Arrange
        CreateTodoItemRequestModel createTodoRequestModel = new CreateTodoItemRequestModel()
        {
            Title = "MyTodoItem",
            Description = "Todo Item Description",
            IsDone = false,
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bogusToken");
        string todoId = _testTodo!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/todos/{todoId}/items", createTodoRequestModel);
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("InvalidAccessToken");
    }

    [Test, Order(2)]
    public async Task CreateTodoItem_ShouldReturnNotFound_IfTodoNotFoundOrDoesNotBelongToUser()
    {
        //Arrange
        CreateTodoItemRequestModel createTodoRequestModel = new CreateTodoItemRequestModel()
        {
            Title = "MyTodoItem",
            Description = "Todo Item Description",
            IsDone = false,
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string bogusTodoId = "bogusTodoId";

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/todos/{bogusTodoId}/items", createTodoRequestModel);

        //Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test, Order(3)]
    public async Task CreateTodo_ShouldSucceedAndReturnTodoAndLocation()
    {
        //Arrange
        CreateTodoItemRequestModel createTodoRequestModel = new CreateTodoItemRequestModel()
        {
            Title = "MyTodoItem",
            Description = "Todo Item Description",
            IsDone = false,
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string todoId = _testTodo!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/todos/{todoId}/items", createTodoRequestModel);
        string? responseBody = await response.Content.ReadAsStringAsync();
        _testTodo = JsonSerializer.Deserialize<Todo>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        _testTodo.Should().NotBeNull();
        _testTodo!.Title.Should().Be("MyTodoItem");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        httpClient.Dispose();
        await ResetDatabaseHelperMethods.ResetNoSqlEmailDatabase();
    }
}
