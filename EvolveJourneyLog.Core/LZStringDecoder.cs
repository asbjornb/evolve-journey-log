using LZStringCSharp;

namespace EvolveJourneyLog.Core;

public class LZStringDecoder
{
    public string Decode(string compressedData)
    {
        var decompressedData = LZString.DecompressFromBase64(compressedData.Trim());
        return decompressedData ?? throw new InvalidFormatException("Failed to decompress the provided data.");
    }
}

public class InvalidFormatException : Exception
{
    public InvalidFormatException()
    {
    }

    public InvalidFormatException(string message)
        : base(message)
    {
    }

    public InvalidFormatException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
