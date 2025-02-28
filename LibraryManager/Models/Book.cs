using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using TkachevProject4.Utils;

namespace TkachevProject4.Models;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public Genre Genre { get; set; }
    public int PublicationYear { get; set; }
    public string ISBN { get; set; }
    
    [JsonProperty("rating")]
    [Range(0, 10)]
    public double Rating { get; set; } = 0;
    public string CoverPath { get; set; }
    
    [JsonProperty("lastReadDate")]
    public DateTime? LastReadDate { get; set; }
    
    public void Validate() 
    {
        if (!ISBNValidator.IsValid(ISBN))
            throw new ArgumentException("Invalid ISBN");
    }
}