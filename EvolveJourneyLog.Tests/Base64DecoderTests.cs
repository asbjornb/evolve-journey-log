using EvolveJourneyLog.Core;
using FluentAssertions;

namespace EvolveJourneyLog.Tests;

public class Base64DecoderTests
{
    [Fact]
    public void Decode_ValidBase64_ReturnsDecodedString()
    {
        var encoder = new Base64Decoder();
        var result = encoder.Decode("SGVsbG8gd29ybGQ=");
        result.Should().Be("Hello world");
    }

    [Fact]
    public void Decode_InvalidBase64_ThrowsValidationException()
    {
        var encoder = new Base64Decoder();
        Assert.Throws<FormatException>(() => encoder.Decode("InvalidBase64!"));
    }
}