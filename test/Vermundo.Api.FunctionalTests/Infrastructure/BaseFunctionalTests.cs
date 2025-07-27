namespace Vermundo.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTests : IClassFixture<FunctionalTestsWebAppFactory>
{
    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTests(FunctionalTestsWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }
}