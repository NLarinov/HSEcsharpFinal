using Refit;
using Spectre.Console;
using TkachevProject4.Models;
using TkachevProject4.Services.Interfaces;
using TkachevProject4.Utils;

namespace TkachevProject4.Services;

public class OpenLibraryService(IOpenLibraryClient client)
{
    public async Task<Book?> EnrichBookInfo(Book book)
    {
        try
        {
            if (!ISBNValidator.IsValid(book.Isbn))
            {
                AnsiConsole.MarkupLine("[red]Невозможно выполнить запрос: неверный ISBN[/]");
                return book;
            }

            var response = await client.GetBookInfo(book.Isbn!);

            if (response.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Информация о книге не найдена[/]");
                return book;
            }

            var bookData = response.First().Value;

            var rnd = new Random();

            return new Book
            {
                Title = !string.IsNullOrEmpty(bookData.Title) ? bookData.Title : book.Title,
                Author = bookData.Authors?.FirstOrDefault()?.Name ??
                    book.Author,
                PublicationYear = int.Parse(bookData.PublishDate ?? "0000"),
                Isbn = book.Isbn,
                Genre = MapGenre(bookData.Subjects),
                CoverPath = bookData.ThumbnailUrl?.Small, // можно выбрать любой размер
                Rating = rnd.Next(3, 5)
            };
        }
        catch (ApiException ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка API: {ex.Message}[/]");
            return book;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}, {ex}[/]");
            return book;
        }
    }

    private static Genre MapGenre(List<Subject?>? genres)
    {
        if (genres == null) return Genre.Other;
        
        foreach (var genre in genres)
        {
            switch (genre?.Name)
            {
                case "Фантастика": return Genre.Fantasy;
                case "Fantasy": return Genre.Fantasy;
                case "Fiction": return Genre.Fantasy;
                case "Детектив": return Genre.Detective;
                case "Исторический": return Genre.History;
                case "History": return Genre.History;
                case "Роман": return Genre.Novel;
                case "Научное": return Genre.Science;
                case "Science": return Genre.Science;
            }
        }
        
        return Genre.Other;
    }
    
    public static async Task<string?> DownloadCover(Book book, string outputDirectory)
    {
        var coverUrl = book.CoverPath;
    
        if (string.IsNullOrEmpty(coverUrl)) return null;

        using var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(coverUrl);
    
        Directory.CreateDirectory(outputDirectory);
        var fileName = $"{book.Title}.jpg";
        var fullPath = Path.Combine(outputDirectory, fileName);
    
        await File.WriteAllBytesAsync(fullPath, imageBytes);
        return fullPath;
    }
}