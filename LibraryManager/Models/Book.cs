using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using TkachevProject4.Utils;

namespace TkachevProject4.Models;

public class Book
{
    public string? Title { get; init; }
    public string? Author { get; init; }
    public Genre Genre { get; init; }
    public int PublicationYear { get; init; }
    public string? Isbn { get; init; }
    
    [JsonProperty("rating")]
    [Range(0, 10)]
    public double Rating { get; set; }
    public string? CoverPath { get; set; }
    
    [JsonProperty("lastReadDate")]
    public DateTime? LastReadDate { get; set; }
    
    public void Validate() 
    {
        if (!ISBNValidator.IsValid(Isbn))
            throw new ArgumentException("Invalid ISBN");
    }
}