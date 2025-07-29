using Vermundo.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Vermundo.Application.IntegrationTests.Infrastructure;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
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
