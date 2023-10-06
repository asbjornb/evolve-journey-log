using System.Text;

namespace EvolveJourneyLog.Core;

public class Base64Decoder
{
    public string Decode(string encodedData)
    {
        var bytes = Convert.FromBase64String(encodedData);
        return Encoding.UTF8.GetString(bytes);
    }
}
