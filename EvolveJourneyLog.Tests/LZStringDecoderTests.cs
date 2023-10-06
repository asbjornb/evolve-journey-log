using EvolveJourneyLog.Core;
using FluentAssertions;

namespace EvolveJourneyLog.Tests;

public class LZStringDecoderTests
{
    private readonly LZStringDecoder _decoder;

    public LZStringDecoderTests()
    {
        _decoder = new LZStringDecoder();
    }

    [Fact]
    public void Decode_InvalidLzString_ThrowsException()
    {
        Assert.Throws<InvalidFormatException>(() => _decoder.Decode("InvalidBase64LZString!"));
    }

    [Fact]
    public void Decode_GameSave_ReturnsExpectedAttributes()
    {
        // Read the encoded string from file
        var encodedData = File.ReadAllText("TestInput/Save0132.txt");

        // Use the decoder
        var decodedData = _decoder.Decode(encodedData);

        // Check if decoded data contains expected attributes
        decodedData.Should().NotBeNullOrEmpty();
        decodedData.Should().Contain("\"start\":");
        decodedData.Should().Contain("\"days\":");
        decodedData.Should().Contain("\"tdays\":");
    }
}