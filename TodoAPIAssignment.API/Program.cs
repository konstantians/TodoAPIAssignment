using TodoAPIAssignment.AuthenticationLibrary;
using TodoAPIAssignment.DataAccessLibrary;

namespace TodoAPIAssignment.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        IConfiguration configuration = builder.Configuration;

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCosmos<AuthDbContext>(connectionString: configuration["CosmosDbConnectionString"]!,
            databaseName: "GlobalDb");
        builder.Services.AddCosmos<DataDbContext>(connectionString: configuration["CosmosDbConnectionString"]!,
            databaseName: "GlobalDb");

        builder.Services.AddScoped<IAuthenticationDataAccess, AuthenticationDataAccess>();
        builder.Services.AddScoped<ITodoItemDataAccess, TodoItemDataAccess>();
        builder.Services.AddScoped<ITodoDataAccess, TodoDataAccess>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}