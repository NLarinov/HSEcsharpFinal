using TkachevProject4.Extensions;
using TkachevProject4.Models;
using TkachevProject4.Utils;

namespace TkachevProject4.Services;

using Spectre.Console;


public class BookDialogService
{
    public static Book EditBook(Book existingBook)
    {
        AnsiConsole.MarkupLine("[underline yellow]Редактирование книги[/]");
    
        var newTitle = AnsiConsole.Prompt(
            new TextPrompt<string?>("Название книги:")
                .DefaultValue(existingBook.Title));
    
        var newAuthor = AnsiConsole.Prompt(
            new TextPrompt<string?>("Автор:")
                .DefaultValue(existingBook.Author));

        var newYear = AnsiConsole.Prompt(
            new TextPrompt<int>("Год издания:")
                .DefaultValue(existingBook.PublicationYear)
                .Validate(year => 
                    year > 0 && year <= DateTime.Now.Year 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("[red]Некорректный год[/]")));

        var newGenre = AnsiConsole.Prompt(
            new SelectionPrompt<Genre>()
                .Title("Выберите жанр:")
                .AddChoices(Enum.GetValues<Genre>())
                .UseConverter(g => g.GetDescription()));

        var newIsbn = AnsiConsole.Prompt(
            new TextPrompt<string?>("ISBN (10 или 13 цифр):")
                .DefaultValue(existingBook.Isbn)
                .Validate(isbn => 
                    ISBNValidator.IsValid(isbn) 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("[red]Неверный формат ISBN[/]")));

        var newRating = AnsiConsole.Prompt(
            new TextPrompt<double>("Оценка (0-10):")
                .DefaultValue(existingBook.Rating)
                .Validate(r => 
                    r is >= 0 and <= 10 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("[red]Допустимый диапазон 0-10[/]")));

        return new Book
        {
            Title = newTitle,
            Author = newAuthor,
            PublicationYear = newYear,
            Genre = newGenre,
            Isbn = newIsbn,
            Rating = newRating,
            CoverPath = existingBook.CoverPath
        };
    }
    
    public static async Task HandleBookCover(Book book, OpenLibraryService olService)
    {
        if (await AnsiConsole.ConfirmAsync("Загрузить обложку из OpenLibrary?"))
        {
            var coversDir = Path.Combine(Environment.CurrentDirectory, "covers");
            var coverPath = await OpenLibraryService.DownloadCover(book, coversDir);
        
            if (!string.IsNullOrEmpty(coverPath))
            {
                book.CoverPath = coverPath;
                AnsiConsole.MarkupLine("[green]Обложка успешно загружена![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Обложка не найдена[/]");
            }
        }
    }
    
    public static Book CreateNewBook()
    {
        AnsiConsole.MarkupLine("[underline green]Добавление новой книги[/]");
        
        var title = AnsiConsole.Ask<string>("Название книги:");
        var author = AnsiConsole.Ask<string>("Автор:");
        var year = AnsiConsole.Prompt(
            new TextPrompt<int>("Год издания:")
                .Validate(year => 
                    year > 0 && year <= DateTime.Now.Year 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("[red]Некорректный год[/]")));
        
        var genre = AnsiConsole.Prompt(
            new SelectionPrompt<Genre>()
                .Title("Выберите жанр:")
                .AddChoices(Enum.GetValues<Genre>())
                .UseConverter(g => g.GetDescription()));
        
        var isbn = AnsiConsole.Prompt(
            new TextPrompt<string?>("ISBN (10 или 13 цифр):")
                .Validate(isbn => 
                    ISBNValidator.IsValid(isbn) 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("[red]Неверный формат ISBN[/]")));

        return new Book
        {
            Title = title,
            Author = author,
            PublicationYear = year,
            Genre = genre,
            Isbn = isbn
        };
    }
}
