using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
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
public class TodosControllerTests
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
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bogusToken");

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/todos", createTodoRequestModel);
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
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        //Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/todos", createTodoRequestModel);
        string? responseBody = await response.Content.ReadAsStringAsync();
        _testTodo = JsonSerializer.Deserialize<Todo>(responseBody, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true})!;

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        _testTodo.Should().NotBeNull();
        _testTodo!.Title.Should().Be("MyTodo");
    }

    [Test, Order(3)]
    public async Task GetTodos_ShouldReturnInvalidTokenError_IfTokenNotValid()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bogusToken");

        //Act
        HttpResponseMessage response = await httpClient.GetAsync("api/todos");
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("InvalidAccessToken");
    }

    [Test, Order(4)]
    public async Task GetTodos_ShouldReturnOkAndTodos()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        //Act
        HttpResponseMessage response = await httpClient.GetAsync("api/todos");
        string? responseBody = await response.Content.ReadAsStringAsync();
        List<Todo>? todos = JsonSerializer.Deserialize<List<Todo>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todos.Should().NotBeNull();
        todos.Should().HaveCount(1);
        todos!.FirstOrDefault()!.Title.Should().Be("MyTodo");
    }

    [Test, Order(5)]
    public async Task GetTodo_ShouldReturnInvalidTokenError_IfTokenNotValid()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bogusToken");
        string todoId = _testTodo!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.GetAsync($"api/todos/{todoId}");
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("InvalidAccessToken");
    }

    [Test, Order(6)]
    public async Task GetTodo_ShouldReturnNotFound_IfTodoNotFoundOrDoesNotBelongToUser()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string bogusTodoId = "bogusTodoId";

        //Act
        HttpResponseMessage response = await httpClient.GetAsync($"api/todos/{bogusTodoId}");

        //Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test, Order(7)]
    public async Task GetTodo_ShouldReturnOkAndTodo()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string todoId = _testTodo!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.GetAsync($"api/todos/{todoId}");
        string? responseBody = await response.Content.ReadAsStringAsync();
        Todo? todo = JsonSerializer.Deserialize<Todo>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(_testTodo.Id);
    }

    [Test, Order(8)]
    public async Task UpdateTodo_ShouldReturnInvalidTokenError_IfTokenNotValid()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bogusToken");
        UpdateTodoRequestModel updateTodoRequestModel = new UpdateTodoRequestModel() { 
            Id = _testTodo!.Id!, 
            Title = "TodoUpdatedTitle",
            IsDone = true
        };

        //Act
        HttpResponseMessage response = await httpClient.PutAsJsonAsync("api/todos", updateTodoRequestModel);
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("InvalidAccessToken");
    }

    [Test, Order(9)]
    public async Task UpdateTodo_ShouldReturnNotFound_IfTodoNotFoundOrDoesNotBelongToUser()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        UpdateTodoRequestModel updateTodoRequestModel = new UpdateTodoRequestModel()
        {
            Id = "bogusTodoId",
            Title = "TodoUpdatedTitle",
            IsDone = true
        };

        //Act
        HttpResponseMessage response = await httpClient.PutAsJsonAsync("api/todos", updateTodoRequestModel);

        //Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test, Order(10)]
    public async Task UpdateTodo_ShouldReturnOkAndUpdatedTodo()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string updatedTitle = "TodoUpdatedTitle";
        UpdateTodoRequestModel updateTodoRequestModel = new UpdateTodoRequestModel()
        {
            Id = _testTodo!.Id!,
            Title = updatedTitle,
            IsDone = true
        };

        //Act
        HttpResponseMessage response = await httpClient.PutAsJsonAsync("api/todos", updateTodoRequestModel);
        string? responseBody = await response.Content.ReadAsStringAsync();
        Todo? todo = JsonSerializer.Deserialize<Todo>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(_testTodo.Id);
        todo!.Title.Should().Be(updatedTitle);
    }

    [Test, Order(11)]
    public async Task DeleteTodo_ShouldReturnInvalidTokenError_IfTokenNotValid()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bogusToken");
        string todoId = _testTodo!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.DeleteAsync($"api/todos/{todoId}");
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("InvalidAccessToken");
    }

    [Test, Order(12)]
    public async Task DeleteTodo_ShouldReturnNotFound_IfTodoNotFoundOrDoesNotBelongToUser()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string bogusTodoId = "bogusTodoId";

        //Act
        HttpResponseMessage response = await httpClient.DeleteAsync($"api/todos/{bogusTodoId}");

        //Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test, Order(13)]
    public async Task Delete_ShouldReturnNoContentAndDeleteTodo()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string todoId = _testTodo!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.DeleteAsync($"api/todos/{todoId}");
        HttpResponseMessage responseAfterDeletion = await httpClient.GetAsync($"api/todos/{todoId}");

        //Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        responseAfterDeletion.Should().NotBeNull();
        responseAfterDeletion.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
