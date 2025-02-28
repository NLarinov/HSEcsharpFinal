using Polly;
using Refit;
using TkachevProject4.Models;
using TkachevProject4.Services;
using TkachevProject4.Services.Interfaces;
using TkachevProject4.Utils;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace TkachevProject4;


class Program
{
    static async Task Main()
    {
        try
        {
            var serviceProvider = ConfigureServices();
            var libraryService = serviceProvider.GetRequiredService<ILibraryService>();
            var bookDialog = serviceProvider.GetRequiredService<BookDialogService>();

            AnsiConsole.Clear();
            ConsolePrinter.DisplayHeader();

            await HandleLibraryFileLoading(libraryService);
            await MainApplicationLoop(serviceProvider, libraryService, bookDialog);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Критическая ошибка: {ex.Message}[/]");
            AnsiConsole.MarkupLine("[yellow]Нажмите любую клавишу для выхода...[/]");
            Console.ReadKey();
        }
    }

    private static Task HandleLibraryFileLoading(ILibraryService libraryService)
    {
        var path = AnsiConsole.Ask<string>("[bold]Введите путь к файлу библиотеки:[/]");
        
        if (!File.Exists(path))
        {
            if (AnsiConsole.Confirm("Файл не существует. Создать новую библиотеку?"))
            {
                File.WriteAllText(path, "[]");
            }
            else
            {
                throw new OperationCanceledException();
            }
        }

        libraryService.LoadLibrary(path);
        AnsiConsole.MarkupLine($"[green]Загружено книг: {libraryService.Books.Count}[/]");
        return Task.CompletedTask;
    }

    private static async Task MainApplicationLoop(
        IServiceProvider serviceProvider,
        ILibraryService libraryService,
        BookDialogService bookDialog)
    {
        var olService = serviceProvider.GetRequiredService<OpenLibraryService>();
        var recommendationService = serviceProvider.GetRequiredService<RecommendationService>();
        var filterService = serviceProvider.GetRequiredService<BookFilterService>();
        var importExportService = serviceProvider.GetRequiredService<ImportExportService>();

        while (true)
        {
            AnsiConsole.Clear();
            ConsolePrinter.DisplayHeader();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold green]Главное меню[/]")
                    .PageSize(15)
                    .AddChoices(
                        "Просмотр книг",
                        "Добавить книгу",
                        "Поиск в OpenLibrary",
                        "Редактировать книгу",
                        "Удалить книгу",
                        "Рекомендации",
                        "Календарь изданий",
                        "Импорт данных",
                        "Экспорт данных",
                        "Сохранить изменения",
                        "Выход"
                    ));

            try
            {
                switch (choice)
                {
                    case "Просмотр книг":
                        HandleBookViewing(libraryService, filterService);
                        break;

                    case "Добавить книгу":
                        await HandleBookAddition(libraryService, bookDialog, olService);
                        break;

                    case "Поиск в OpenLibrary":
                        await HandleOpenLibrarySearch(libraryService, bookDialog, olService);
                        break;

                    case "Редактировать книгу":
                        HandleBookEditing(libraryService, bookDialog);
                        break;

                    case "Удалить книгу":
                        HandleBookDeletion(libraryService);
                        break;

                    case "Рекомендации":
                        ShowRecommendations(libraryService, recommendationService);
                        break;

                    case "Календарь изданий":
                        ConsolePrinter.DisplayPublicationCalendar(libraryService.Books);
                        break;

                    case "Импорт данных":
                        await HandleDataImport(importExportService, libraryService);
                        break;

                    case "Экспорт данных":
                        await HandleDataExport(importExportService, libraryService);
                        break;

                    case "Сохранить изменения":
                        libraryService.SaveLibrary();
                        AnsiConsole.MarkupLine("[green]Изменения сохранены![/]");
                        break;

                    case "Выход":
                        libraryService.SaveLibrary();
                        return;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            }

            AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу для продолжения...[/]");
            Console.ReadKey();
        }
    }

    private static void HandleBookViewing(ILibraryService libraryService, BookFilterService filterService)
    {
        var searchTerm = AnsiConsole.Prompt(
            new TextPrompt<string>("Поиск (оставьте пустым для отмены):")
                .AllowEmpty() // Разрешаем пустой ввод
        );

        var genrePrompt = new SelectionPrompt<Genre>()
            .Title("Фильтр по жанру:")
            .AddChoices(Enum.GetValues<Genre>());

        genrePrompt.AddChoice(default);

        var genreChoice = AnsiConsole.Prompt(genrePrompt);

        var sortField = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Сортировать по:")
                .AddChoices("Название", "Автор", "Год", "Рейтинг"));

        var sortDirection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Порядок сортировки:")
                .AddChoices("По возрастанию", "По убыванию"));

        var filteredBooks = filterService.FilterAndSort(
            libraryService.Books,
            searchTerm,
            genreChoice,
            sortField switch
            {
                "Автор" => "Author",
                "Год" => "Year",
                "Рейтинг" => "Rating",
                _ => "Title"
            },
            sortDirection == "По возрастанию"
        );

        ConsolePrinter.DisplayBooksTable(filteredBooks);
    }

    private static async Task HandleBookAddition(
        ILibraryService libraryService,
        BookDialogService bookDialog,
        OpenLibraryService olService)
    {
        var newBook = bookDialog.CreateNewBook();
        await bookDialog.HandleBookCover(newBook, olService);
        libraryService.AddBook(newBook);
        AnsiConsole.MarkupLine("[green]Книга успешно добавлена![/]");
    }

    private static async Task HandleOpenLibrarySearch(
        ILibraryService libraryService,
        BookDialogService bookDialog,
        OpenLibraryService olService)
    {
        var isbn = AnsiConsole.Prompt(
            new TextPrompt<string>("Введите ISBN для поиска:")
                .Validate(i => ISBNValidator.IsValid(i) 
                    ? ValidationResult.Success() 
                    : ValidationResult.Error("Неверный ISBN")));

        var book = new Book { ISBN = isbn };
        var enrichedBook = await olService.EnrichBookInfo(book);

        if (await AnsiConsole.ConfirmAsync("Найдена информация. Использовать её?"))
        {
            await bookDialog.HandleBookCover(enrichedBook, olService);
            libraryService.AddBook(enrichedBook);
            AnsiConsole.MarkupLine("[green]Книга добавлена из OpenLibrary![/]");
        }
    }

    private static void HandleBookEditing(ILibraryService libraryService, BookDialogService bookDialog)
    {
        var index = ConsoleSelector.SelectBookIndex(libraryService.Books);
        if (index == -1) return;

        var updatedBook = bookDialog.EditBook(libraryService.Books[index]);
        libraryService.UpdateBook(index, updatedBook);
        AnsiConsole.MarkupLine("[green]Книга успешно обновлена![/]");
    }

    private static void HandleBookDeletion(ILibraryService libraryService)
    {
        var index = ConsoleSelector.SelectBookIndex(libraryService.Books);
        if (index == -1) return;

        if (!AnsiConsole.Confirm("Вы уверены, что хотите удалить эту книгу?")) return;
        libraryService.RemoveBook(index);
        AnsiConsole.MarkupLine("[green]Книга удалена![/]");
    }

    private static void ShowRecommendations(
        ILibraryService libraryService,
        RecommendationService recommendationService)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Тип рекомендаций:")
                .AddChoices("Топ рейтинга", "Персональные", "Назад"));

        if (choice == "Назад") return;

        var books = choice switch
        {
            "Топ рейтинга" => recommendationService.GetTopRatedBooks(libraryService.Books),
            "Персональные" => recommendationService.GetPersonalizedRecommendations(
                GetUserRatedBooks(),
                libraryService.Books),
            _ => []
        };

        ConsolePrinter.DisplayBooksTable(books);
    }

    private static Task HandleDataImport(
        ImportExportService importExportService,
        ILibraryService libraryService)
    {
        var path = AnsiConsole.Ask<string>("Введите путь к файлу для импорта:");
        var importedBooks = importExportService.ImportFromFile(path);
        libraryService.Books.AddRange(importedBooks);
        AnsiConsole.MarkupLine($"[green]Импортировано {importedBooks.Count} книг[/]");
        return Task.CompletedTask;
    }

    private static Task HandleDataExport(
        ImportExportService importExportService,
        ILibraryService libraryService)
    {
        var path = AnsiConsole.Ask<string>("Введите путь для экспорта:");
        importExportService.ExportToFile(libraryService.Books, path);
        AnsiConsole.MarkupLine("[green]Экспорт завершён успешно![/]");
        return Task.CompletedTask;
    }

    private static List<Book> GetUserRatedBooks()
    {
        // Заглушка
        return [];
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILibraryService, LibraryService>();
        services.AddSingleton<BookDialogService>();
        services.AddSingleton<BookFilterService>();
        services.AddSingleton<ImportExportService>();
        services.AddSingleton<RecommendationService>();
        services.AddSingleton<OpenLibraryService>();

        services.AddRefitClient<IOpenLibraryClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://openlibrary.org"))
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

        return services.BuildServiceProvider();
    }
}
