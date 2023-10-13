using System.Reflection;
using Newtonsoft.Json.Linq;

namespace EvolveJourneyLog.Core.JsonExtraction;
internal static class JsonFlattener
{
    //Consider caching most recently seen save with the flattened version (for multiple model extractions on the same file to avoid re-flattening)
    //Consider caching reflection results for each model type to avoid redoing them
    public static T DeSerialize<T>(string json) where T : new()
    {
        var token = JToken.Parse(json);
        var dict = FlattenJson(token);
        return PopulateFromDictionary<T>(dict);
    }

    private static T PopulateFromDictionary<T>(Dictionary<string, object> dict) where T : new()
    {
        var obj = new T();
        var type = typeof(T);

        foreach (var prop in type.GetProperties())
        {
            var attr = prop.GetCustomAttribute<PathAttribute>();
            if (attr != null && dict.ContainsKey(attr.Path))
            {
                prop.SetValue(obj, Convert.ChangeType(dict[attr.Path], prop.PropertyType));
            }
        }

        return obj;
    }

    private static Dictionary<string, object> FlattenJson(JToken token, string prefix = "")
    {
        var dict = new Dictionary<string, object>();
        if (token.Type == JTokenType.Object)
        {
            foreach (var child in token.Children<JProperty>())
            {
                var childContent = FlattenJson(child.Value, $"{prefix}{child.Name}/");
                foreach (var entry in childContent)
                {
                    dict[entry.Key] = entry.Value;
                }
            }
        }
        else if (token.Type == JTokenType.Array)
        {
            int index = 0;
            foreach (var child in token.Children())
            {
                var childContent = FlattenJson(child, $"{prefix}[{index++}]/");
                foreach (var entry in childContent)
                {
                    dict[entry.Key] = entry.Value;
                }
            }
        }
        else
        {
            if (token is JValue jValue && jValue.Value is not null)
            {
                dict[prefix.TrimEnd('/')] = jValue.Value;
            }
        }
        return dict;
    }
}
