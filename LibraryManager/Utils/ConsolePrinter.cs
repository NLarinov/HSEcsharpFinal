using TkachevProject4.Extensions;
using TkachevProject4.Models;

namespace TkachevProject4.Utils;

using Spectre.Console;
using System.Collections.Generic;

public static class ConsolePrinter
{
    public static void DisplayHeader()
    {
        AnsiConsole.Write(
            new FigletText("Library Manager")
                .Centered()
                .Color(Color.Yellow));
    
        AnsiConsole.Write(new Rule("[yellow]Управление вашей библиотекой[/]"));
    }
    
    public static void DisplayBookDetails(Book book)
    {
        var panel = new Panel(
            new Rows(
                new Text($"Автор: {book.Author}"),
                new Text($"Жанр: {book.Genre.GetDescription()}"),
                new Text($"Год издания: {book.PublicationYear}"),
                new Text($"ISBN: {book.ISBN}"),
                new Text($"Рейтинг: {book.Rating:0.0}")
            ))
        {
            Header = new PanelHeader($"[bold yellow]{book.Title.EscapeMarkup()}[/]"),
            Border = BoxBorder.Rounded
        };

        if (!string.IsNullOrEmpty(book.CoverPath))
        {
            try
            {
                ImageHelper.DisplayBookCover(book.CoverPath);
            }
            catch
            {
                AnsiConsole.MarkupLine("[red]Ошибка загрузки обложки[/]");
            }
        }

        AnsiConsole.Write(panel);
    }
    
    public static void DisplayPublicationCalendar(IEnumerable<Book> books)
    {
        var years = books
            .GroupBy(b => b.PublicationYear)
            .ToDictionary(g => g.Key, g => g.Count());

        var minYear = years.Keys.Min();
        var maxYear = years.Keys.Max();
        var maxCount = years.Values.Max();

        var calendar = new Calendar(new DateTime(minYear, 1, 1))
            .HighlightStyle(new Style(Color.Yellow, decoration: Decoration.Bold))
            .HeaderStyle(new Style(Color.Cyan1, decoration: Decoration.Bold));

        foreach (var year in years)
        {
            var intensity = (double)year.Value / maxCount;
            var color = new Color(
                (byte)(255 * intensity),
                (byte)(128 * (1 - intensity)),
                0);

            for (int month = 1; month <= 12; month++)
            {
                calendar.AddCalendarEvent(new DateTime(year.Key, month, 1), new Style(color));
            }
        }

        AnsiConsole.Write(calendar);
    }
    
    public static void DisplayBooksTable(IEnumerable<Book> books)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]БИБЛИОТЕКА[/]")
            .AddColumn(new TableColumn("[bold]Название[/]").Centered())
            .AddColumn(new TableColumn("[bold]Автор[/]").Centered())
            .AddColumn(new TableColumn("[bold]Жанр[/]").Centered())
            .AddColumn(new TableColumn("[bold]Год[/]").Centered())
            .AddColumn(new TableColumn("[bold]ISBN[/]").Centered());

        foreach (var book in books)
        {
            table.AddRow(
                Markup.Escape(book.Title),
                Markup.Escape(book.Author),
                book.Genre.GetDescription(),
                book.PublicationYear.ToString(),
                Markup.Escape(book.ISBN)
            );
        }

        AnsiConsole.Write(table);
    }
}
