using Bogus; 

namespace Vermundo.Application.UnitTests.UpdateArticle;

public class UpdateArticleCommandValidatorTests 
{
    private readonly UpdateArticleCommandValidator _validator = new();
    private readonly Faker _faker = new();

    [Fact]
    public async Task Validate_ValidGuid_ReturnsSuccess()
    {
        var command = new UpdateArticleCommand(Guid.NewGuid(), _faker.Image.PicsumUrl());
        Console.WriteLine(_faker.Image.PicsumUrl());
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

        [Fact]
    public async Task Validate_EmptyGuid_ReturnsFailure()
    {
        var command = new UpdateArticleCommand(Guid.Empty, _faker.Image.PicsumUrl());
        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateArticleCommand.Id));
    }

    [Fact]
    public async Task Validate_NullImageUrl_ReturnsFailure()
    {
        var command = new UpdateArticleCommand(Guid.NewGuid(), null!);
        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateArticleCommand.ImageUrl));
    }

    [Fact]
    public async Task Validate_EmptyImageUrl_ReturnsFailure()
    {
        var command = new UpdateArticleCommand(Guid.NewGuid(), string.Empty);
        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateArticleCommand.ImageUrl));
    }

    [Fact]
    public async Task Validate_InvalidImageUrl_ReturnsFailure()
    {
        var command = new UpdateArticleCommand(Guid.NewGuid(), "not-a-valid-url");
        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateArticleCommand.ImageUrl));
    }
}
