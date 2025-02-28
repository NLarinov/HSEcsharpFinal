using Spectre.Console;
using TkachevProject4.Models;

namespace TkachevProject4.Utils;

public static class ConsoleSelector
{
    public static int SelectBookIndex(IList<Book> books, string title = "Выберите книгу")
    {
        var choices = books.Select((b, i) => 
                $"{i + 1}. [bold]{b.Title}[/] ({b.Author}, {b.PublicationYear})")
            .ToList();
            
        choices.Add("[red]Отмена[/]");

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(15)
                .AddChoices(choices));

        int selectedIndex = choices.IndexOf(selection);
        return selectedIndex < books.Count ? selectedIndex : -1;
    }
}