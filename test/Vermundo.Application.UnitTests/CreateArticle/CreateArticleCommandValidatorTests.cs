using Bogus;
using Vermundo.Application.Articles;
using Vermundo.TestUtils;

namespace Vermundo.Application.UnitTests.CreateArticle;

public class CreateArticleCommandValidatorTests
{
    private readonly CreateArticleCommandValidator _validator = new();
    private readonly CreateArticleCommandFactory _factory = new();
    private readonly Faker _faker = new();

    private const int MaxImageUrlLength = 512;

    [Fact]
    public async Task Validate_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = _factory.Create(_faker.Lorem.Sentence(3), _faker.Lorem.Paragraphs(3));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyTitle_ReturnsFailure()
    {
        var command = _factory.Create("", _faker.Lorem.Paragraph());

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_TooLongTitle_ReturnsFailure()
    {
        var longTitle = _faker.Random.String2(101);
        var command = _factory.Create(longTitle, _faker.Lorem.Paragraph());

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_TooShortTitle_ReturnsFailure()
    {
        var shortTitle = _faker.Random.String2(4);
        var command = _factory.Create(shortTitle, _faker.Lorem.Paragraph());

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_EmptyBody_ReturnsFailure()
    {
        var command = _factory.Create(_faker.Lorem.Sentence(), "");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Body");
    }

    [Fact]
    public async Task Validate_TooLongBody_ReturnsFailure()
    {
        var longBody = _faker.Random.String2(10_001);
        var command = _factory.Create(_faker.Lorem.Sentence(), longBody);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Body");
    }

    [Fact]
    public async Task Validate_TooShortBody_ReturnsFailure()
    {
        var shortbody = _faker.Random.String2(99);
        var command = _factory.Create(_faker.Lorem.Sentence(), shortbody);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Body");
    }

    [Fact]
    public async Task Validate_ContainsScriptTag_ReturnsFailure()
    {
        var scriptBody = "<script>alert('XSS');</script>" + _faker.Lorem.Paragraph();
        var command = _factory.Create(_faker.Lorem.Sentence(), scriptBody);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Body");
    }

    [Fact]
    public async Task Validate_TooLongImageUrl_ReturnsFailure()
    {
        var longImageUrl = _faker.Random.String2(MaxImageUrlLength + 1);
        var command = _factory.Create(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            longImageUrl
        );

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e =>
                e.PropertyName == "ImageUrl"
                && e.ErrorMessage.Contains("characters", StringComparison.OrdinalIgnoreCase)
        );
    }

    [Fact]
    public async Task Validate_ImageUrlIsNotUrl_ReturnsFailure()
    {
        var invalidUrl = _faker.Random.String2(50);
        var command = _factory.Create(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            invalidUrl
        );

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e =>
                e.PropertyName == "ImageUrl"
                && e.ErrorMessage.Contains("valid URL", StringComparison.OrdinalIgnoreCase)
        );
    }

    [Fact]
    public async Task Validate_ImageUrlIsValid_ReturnsSuccess()
    {
        var validUrl = _faker.Internet.Url();
        var command = _factory.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(), validUrl);

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_ImageUrlIsNull_ReturnsSuccess()
    {
        var command = _factory.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(), null);

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }
}
