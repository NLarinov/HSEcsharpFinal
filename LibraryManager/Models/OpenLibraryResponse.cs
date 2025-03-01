using Newtonsoft.Json;
using TkachevProject4.Services;

namespace TkachevProject4.Models;

public class OpenLibraryResponse
{
    [JsonProperty("title")]
    public string? Title { get; set; }
    
    [JsonProperty("authors")]
    public List<AuthorInfo?>? Authors { get; set; }
    
    [JsonProperty("publish_date")]
    public string? PublishDate { get; set; }
    
    [JsonProperty("cover")]
    public Cover? ThumbnailUrl { get; set; }
    
    [JsonProperty("subjects")]
    [JsonConverter(typeof(SubjectConverter))]
    public List<Subject?>? Subjects { get; set; }
}

public class Cover
{
    [JsonProperty("small")]
    public string? Small { get; set; }
    
    [JsonProperty("medium")]
    public string? Medium { get; set; }
    
    [JsonProperty("large")]
    public string? Large { get; set; }
}

public class Subject
{
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("url")]
    public string? Url { get; set; }
}

public class AuthorInfo
{
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("url")]
    public string? Key { get; set; }
}