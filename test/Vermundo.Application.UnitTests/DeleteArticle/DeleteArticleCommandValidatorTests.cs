using Bogus;
using Vermundo.Application.Articles;

namespace Vermundo.Application.UnitTests.DeleteArticle;

public class DeleteArticleCommandValidatorTests
{
    private readonly DeleteArticleCommandValidator _validator = new();
    private readonly Faker _faker = new();

    [Fact]
    public async Task Validate_ValidGuid_ReturnsSuccess()
    {
        var command = new DeleteArticleCommand(Guid.NewGuid());
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyGuid_ReturnsFailure()
    {
        var command = new DeleteArticleCommand(Guid.Empty);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")] 
    [InlineData(null)]
    public async Task Validate_WithKnownBadGuids_ReturnsInvalid(string? guidString)
    {
        var guid = guidString is null ? Guid.Empty : Guid.Parse(guidString);
        var query = new DeleteArticleCommand(guid);
        var result = await _validator.ValidateAsync(query);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }
}
