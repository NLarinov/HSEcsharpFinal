using Newtonsoft.Json;

namespace TkachevProject4.Models;

public class OpenLibraryResponse
{
    [JsonProperty("title")]
    public string Title { get; set; }
    
    [JsonProperty("authors")]
    public List<AuthorInfo> Authors { get; set; }
    
    [JsonProperty("publish_date")]
    public string PublishDate { get; set; }
    
    [JsonProperty("cover")]
    public CoverInfo Cover { get; set; }
}

public class AuthorInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }
}

public class CoverInfo
{
    [JsonProperty("medium")]
    public string MediumUrl { get; set; }
}