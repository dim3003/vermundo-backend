using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Vermundo.Application.Abstractions;

namespace Vermundo.Infrastructure.Email;

public sealed class SecureConfirmationTokenGenerator : IConfirmationTokenGenerator
{
    private readonly int _byteLength;

    // 32 bytes ≈ 256 bits of entropy; very safe for confirmation tokens
    public SecureConfirmationTokenGenerator(int byteLength = 32)
    {
        if (byteLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(byteLength), "Byte length must be positive.");

        _byteLength = byteLength;
    }

    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(_byteLength);

        // URL-safe Base64 without padding: only [A-Za-z0-9-_]
        var token = WebEncoders.Base64UrlEncode(bytes);

        return token;
    }
}
