using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TkachevProject4.Models;

namespace TkachevProject4.Services;

public class SubjectConverter : JsonConverter<List<Subject>>
{
    public override List<Subject> ReadJson(
        JsonReader reader, 
        Type objectType, 
        List<Subject>? existingValue, 
        bool hasExistingValue, 
        JsonSerializer serializer)
    {
        var result = new List<Subject>();
        var token = JToken.Load(reader);

        if (token.Type != JTokenType.Array) return result;
        foreach (var item in token)
        {
            switch (item.Type)
            {
                case JTokenType.String:
                    result.Add(new Subject { Name = item.Value<string>() });
                    break;
                case JTokenType.Object:
                    result.Add(item.ToObject<Subject>() ?? new Subject { Name = item.Value<string>() });
                    break;
            }
        }

        return result;
    }

    public override void WriteJson(
        JsonWriter writer, 
        List<Subject>? value, 
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}