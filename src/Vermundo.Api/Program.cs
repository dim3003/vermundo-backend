using Vermundo.Api.Controllers.Articles;
using Vermundo.Api.Extensions;
using Vermundo.Application;
using Vermundo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


// Configure Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowLocalhost5173");

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
routeGroupBuilder.MapNewsletterEndpoints();

app.Run();

public partial class Program;
