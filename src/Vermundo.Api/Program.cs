using Vermundo.Api.Extensions;
using Vermundo.Application;
using Vermundo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
app.MapOpenApi();
app.ApplyMigrations();
//}

app.UseHttpsRedirection();
app.Run();

public partial class Program;
