using Vermundo.Infrastructure.Email;

namespace Vermundo.Infrastructure.UnitTests.Email;

public class SecureConfirmationTokenGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturn_NonEmptyString()
    {
        // Arrange
        var generator = new SecureConfirmationTokenGenerator();

        // Act
        var token = generator.Generate();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void Generate_ShouldReturn_UrlSafeBase64Token()
    {
        // Arrange
        var generator = new SecureConfirmationTokenGenerator();

        // Act
        var token = generator.Generate();

        // Assert
        Assert.DoesNotContain("+", token);
        Assert.DoesNotContain("/", token);
        Assert.DoesNotContain("=", token); // no padding
    }

    [Fact]
    public void Generate_ShouldProduceDifferentTokens_ForMultipleCalls()
    {
        // Arrange
        var generator = new SecureConfirmationTokenGenerator();

        // Act
        var tokens = Enumerable.Range(0, 100)
            .Select(_ => generator.Generate())
            .ToList();

        // Assert
        // If this ever fails, either the generator is broken
        // or you hit a ~lottery-level collision.
        Assert.Equal(tokens.Count, tokens.Distinct().Count());
    }

    [Fact]
    public void Generate_ShouldRespectConfiguredLength_InCharacters_Minimum()
    {
        // This is optional, but nice if you allow configuration
        var generator = new SecureConfirmationTokenGenerator(byteLength: 32);

        var token = generator.Generate();

        Assert.True(token.Length >= 32, $"Token too short: {token.Length}");
    }
}
