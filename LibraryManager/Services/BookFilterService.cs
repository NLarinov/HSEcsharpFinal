using TkachevProject4.Models;

namespace TkachevProject4.Services;

public class BookFilterService
{
    public IEnumerable<Book> FilterAndSort(
        IEnumerable<Book> books,
        string searchTerm = "",
        Genre? genre = null,
        string sortBy = "Title",
        bool ascending = true)
    {
        var query = books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(b => 
                b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        if (genre != null)
            query = query.Where(b => b.Genre == genre);

        return SortBooks(query, sortBy, ascending);
    }

    private IQueryable<Book> SortBooks(
        IQueryable<Book> query,
        string sortBy,
        bool ascending)
    {
        return sortBy.ToLower() switch
        {
            "author" => ascending 
                ? query.OrderBy(b => b.Author) 
                : query.OrderByDescending(b => b.Author),
            "year" => ascending 
                ? query.OrderBy(b => b.PublicationYear) 
                : query.OrderByDescending(b => b.PublicationYear),
            "rating" => ascending 
                ? query.OrderBy(b => b.Rating) 
                : query.OrderByDescending(b => b.Rating),
            _ => ascending 
                ? query.OrderBy(b => b.Title) 
                : query.OrderByDescending(b => b.Title)
        };
    }
}