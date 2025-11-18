using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vermundo.Api.TestUtils;
using Vermundo.Infrastructure;

namespace Vermundo.Application.IntegrationTests.Infrastructure;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly ApplicationDbContext DbContext;
    protected readonly SpyNewsletterClient _spy;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _spy = factory.Services.GetRequiredService<SpyNewsletterClient>();
    }

    public virtual async Task InitializeAsync()
    {
        DbContext.Articles.RemoveRange(DbContext.Articles);
        await DbContext.SaveChangesAsync();
        _spy.Calls.Clear();
        _spy.FailWith = null;
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
