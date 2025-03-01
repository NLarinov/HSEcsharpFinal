using TkachevProject4.Models;

namespace TkachevProject4.Services;

public class RecommendationService
{
    public List<Book> GetTopRatedBooks(IEnumerable<Book> books, int count = 5)
    {
        return books
            .OrderByDescending(b => b.Rating)
            .ThenBy(b => b.Title)
            .Take(count)
            .ToList();
    }

    public List<Book> GetPersonalizedRecommendations(
        IEnumerable<Book> userRatedBooks, 
        IEnumerable<Book> allBooks,
        int count = 5)
    {
        var ratedBooks = userRatedBooks.ToList();
        var favoriteGenres = ratedBooks
            .GroupBy(b => b.Genre)
            .OrderByDescending(g => g.Average(b => b.Rating))
            .Select(g => g.Key)
            .Take(2);
        
        return allBooks
            .Where(b => ratedBooks.All(ub => ub.Isbn != b.Isbn))
            .Where(b => favoriteGenres.Contains(b.Genre))
            .OrderByDescending(b => b.Rating)
            .Take(count)
            .ToList();
    }
}