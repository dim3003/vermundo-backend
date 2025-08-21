using Vermundo.Api.Controllers.Articles;
using Vermundo.Api.Extensions;
using Vermundo.Application;
using Vermundo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


// Configure Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

// Dev only middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.ApplyMigrations();
    await app.SeedData();
}

// Middleware
app.UseHttpsRedirection();
app.UseCustomExceptionHandler();

// Endpoints
var routeGroupBuilder = app.MapGroup("api/");
routeGroupBuilder.MapArticleEndpoints();

app.Run();

public partial class Program;
