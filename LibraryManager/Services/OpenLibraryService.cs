using TkachevProject4.Models;
using TkachevProject4.Services.Interfaces;

namespace TkachevProject4.Services;

public class OpenLibraryService(IOpenLibraryClient client)
{
    public async Task<Book> EnrichBookInfo(Book book)
    {
        try
        {
            var response = await client.GetBookInfo(book.ISBN);
            if (!response.Any()) return book;

            var data = response.First().Value;
                
            // Обновляем информацию
            book.Title = data.Title ?? book.Title;
            book.Author = data.Authors?.FirstOrDefault()?.Name ?? book.Author;
                
            if (int.TryParse(data.PublishDate as string, out int year)) // ??????????????
                book.PublicationYear = year;

            book.CoverPath = data.Cover?.MediumUrl;

            return book;
        }
        catch
        {
            return book;
        }
    }
    
    public async Task<string> DownloadCover(string isbn, string outputDirectory)
    {
        try
        {
            var response = await client.GetBookInfo(isbn);
            if (!response.Any()) return null;

            var coverUrl = response.First().Value.Cover?.MediumUrl;
            if (string.IsNullOrEmpty(coverUrl)) return null;

            using var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(coverUrl);
        
            Directory.CreateDirectory(outputDirectory);
            var fileName = $"{isbn}.jpg";
            var fullPath = Path.Combine(outputDirectory, fileName);
        
            await File.WriteAllBytesAsync(fullPath, imageBytes);
            return fullPath;
        }
        catch
        {
            return null;
        }
    }
}