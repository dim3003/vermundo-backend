using Microsoft.Extensions.DependencyInjection;
using Vermundo.Api.TestUtils;
using Vermundo.Infrastructure;

namespace Vermundo.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTests : IClassFixture<FunctionalTestsWebAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    protected readonly ApplicationDbContext DbContext;
    protected readonly HttpClient HttpClient;
    protected readonly SpyNewsletterClient _spy;

    protected BaseFunctionalTests(FunctionalTestsWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        HttpClient = factory.CreateClient();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _spy = factory.Services.GetRequiredService<SpyNewsletterClient>();
    }

    public virtual async Task InitializeAsync()
    {
        DbContext.Articles.RemoveRange(DbContext.Articles);
        await DbContext.SaveChangesAsync();
        _spy.Reset();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
