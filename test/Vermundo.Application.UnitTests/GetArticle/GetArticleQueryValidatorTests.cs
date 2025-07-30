using Bogus;
using Vermundo.Application.Articles;

namespace Vermundo.Application.UnitTests.GetArticle;

public class GetArticleQueryValidatorTests
{
    private readonly GetArticleQueryValidator _validator = new();
    private readonly Faker _faker = new();

    [Fact]
    public async Task Validate_WithValidGuid_ReturnsValid()
    {
        var query = new GetArticleQuery(_faker.Random.Guid());
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithEmptyGuid_ReturnsInvalid()
    {
        var query = new GetArticleQuery(Guid.Empty);
        var result = await _validator.ValidateAsync(query);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithNewGuid_ReturnsValid()
    {
        var query = new GetArticleQuery(Guid.NewGuid());
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")] 
    [InlineData(null)]
    public async Task Validate_WithKnownBadGuids_ReturnsInvalid(string? guidString)
    {
        var guid = guidString is null ? Guid.Empty : Guid.Parse(guidString);
        var query = new GetArticleQuery(guid);
        var result = await _validator.ValidateAsync(query);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithDuplicateGuid_ReturnsValid_ButShouldBeHandledByHandlerNotValidator()
    {
        // This test illustrates that validator doesn't check logical consistency like duplicates.
        var existingGuid = Guid.Parse("de305d54-75b4-431b-adb2-eb6b9e546014");
        var query = new GetArticleQuery(existingGuid);
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
    }
}

