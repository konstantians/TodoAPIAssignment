﻿using FluentAssertions;
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
    private TodoItem? _testTodoItem;

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
    public async Task CreateTodoItem_ShouldSucceedAndReturnTodoItemAndLocation()
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
        _testTodoItem = JsonSerializer.Deserialize<TodoItem>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        _testTodoItem.Should().NotBeNull();
        _testTodoItem!.Title.Should().Be("MyTodoItem");
    }

    [Test, Order(4)]
    public async Task GetTodoItem_ShouldReturnInvalidTokenError_IfTokenNotValid()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bogusToken");
        string todoId = _testTodo!.Id!;
        string todoItemId = _testTodoItem!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.GetAsync($"api/todos/{todoId}/items/{todoItemId}");
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("InvalidAccessToken");
    }

    [Test, Order(5)]
    public async Task GetTodoItem_ShouldReturnTodoNotFound_IfTodoNotFoundOrDoesNotBelongToUser()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string bogusTodoId = "bogusTodoId";
        string todoItemId = _testTodoItem!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.GetAsync($"api/todos/{bogusTodoId}/items/{todoItemId}");
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("TodoNotFound");
    }

    [Test, Order(6)]
    public async Task GetTodoItem_ShouldReturnTodoItemNotFound_IfUserTodoExistsButTodoItemDoesNot()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string todoId = _testTodo!.Id!;
        string bogusTodoItemId = "bogusTodoItemId";


        //Act
        HttpResponseMessage response = await httpClient.GetAsync($"api/todos/{todoId}/items/{bogusTodoItemId}");
        string? errorMessage = await JsonParsingHelperMethods.GetSingleStringValueFromBody(response, "errorMessage");

        //Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        errorMessage.Should().NotBeNull();
        errorMessage.Should().Be("TodoItemNotFound");
    }

    [Test, Order(7)]
    public async Task GetTodoItem_ShouldSucceedAndReturnTodoItem()
    {
        //Arrange
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        string todoId = _testTodo!.Id!;
        string todoItemId = _testTodoItem!.Id!;

        //Act
        HttpResponseMessage response = await httpClient.GetAsync($"api/todos/{todoId}/items/{todoItemId}");
        string? responseBody = await response.Content.ReadAsStringAsync();
        TodoItem todoItem = JsonSerializer.Deserialize<TodoItem>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todoItem.Should().NotBeNull();
        todoItem.Id.Should().Be(_testTodoItem!.Id!);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        httpClient.Dispose();
        await ResetDatabaseHelperMethods.ResetNoSqlEmailDatabase();
    }
}
