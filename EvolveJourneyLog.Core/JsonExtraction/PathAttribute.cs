namespace EvolveJourneyLog.Core.JsonExtraction;

[AttributeUsage(AttributeTargets.Property)]
public class PathAttribute : Attribute
{
    public string Path { get; }

    public bool IsRequired { get; set; } = false;

    public PathAttribute(string path)
    {
        Path = path;
    }
}
