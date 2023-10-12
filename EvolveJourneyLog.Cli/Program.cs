using CommandLine;
using EvolveJourneyLog.Core;
using Newtonsoft.Json.Linq;

namespace EvolveJourneyLog.Cli;

// Run with e.g. dotnet run -- -p .\..\EvolveJourneyLog.Tests\TestInput -i
public static class Program
{
    public class Options
    {
        [Option('p', "path", Required = true, HelpText = "Path to the directory containing the .txt files.")]
        public string Path { get; set; }

        [Option('o', "overwrite", Default = false, HelpText = "Overwrite files in the output directory.")]
        public bool Overwrite { get; set; }

        [Option('i', "inspect", Default = false, HelpText = "Inspect JSON and generate a schema.txt file.")]
        public bool Inspect { get; set; }
    }

    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
               .WithParsed(Run);
    }

    private static void Run(Options opts)
    {
        var outputPath = Path.Combine(opts.Path, "output");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        var uniqueKeys = new HashSet<string>();

        foreach (var file in Directory.GetFiles(opts.Path, "*.txt"))
        {
            var content = File.ReadAllText(file);
            var decompressedContent = Decompress(content);

            if (opts.Inspect)
            {
                var jsonObject = JObject.Parse(decompressedContent);
                ExtractKeys(jsonObject, uniqueKeys);
            }

            var outputFilePath = Path.Combine(outputPath, Path.GetFileName(file));
            if (opts.Overwrite || !File.Exists(outputFilePath))
            {
                File.WriteAllText(outputFilePath, decompressedContent);
            }
        }

        if (opts.Inspect)
        {
            File.WriteAllLines(Path.Combine(outputPath, "schema.txt"), uniqueKeys);
        }
    }

    private static string Decompress(string content)
    {
        var decoder = new LZStringDecoder();
        return decoder.Decode(content);
    }

    private static void ExtractKeys(JToken token, HashSet<string> keys, string parent = "")
    {
        if (token is JObject jObject)
        {
            foreach (var kvp in jObject)
            {
                var currentKey = string.IsNullOrEmpty(parent) ? kvp.Key : $"{parent}.{kvp.Key}";
                keys.Add(currentKey);
                ExtractKeys(kvp.Value, keys, currentKey);
            }
        }
        else if (token is JArray jArray)
        {
            foreach (var item in jArray)
            {
                ExtractKeys(item, keys, parent);
            }
        }
    }
}
