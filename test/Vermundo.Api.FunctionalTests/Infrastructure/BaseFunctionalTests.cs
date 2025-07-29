using Microsoft.Extensions.DependencyInjection;
using Vermundo.Infrastructure;

namespace Vermundo.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTests : IClassFixture<FunctionalTestsWebAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext DbContext;
    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTests(FunctionalTestsWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        HttpClient = factory.CreateClient();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public virtual async Task InitializeAsync()
    {
        DbContext.Articles.RemoveRange(DbContext.Articles);
        await DbContext.SaveChangesAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
