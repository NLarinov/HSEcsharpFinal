using System.Globalization;
using TkachevProject4.Services.Interfaces;

namespace TkachevProject4.Services;

using Models;

using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;


public class LibraryService : ILibraryService
{
    private string _currentFilePath;
    public List<Book> Books { get; private set; } = [];

    public void LoadLibrary(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл библиотеки не найден");

        var json = File.ReadAllText(filePath);
        Books = JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
        _currentFilePath = filePath;
    }

    public void SaveLibrary()
    {
        if (string.IsNullOrEmpty(_currentFilePath))
            throw new InvalidOperationException("Путь к файлу не установлен");

        var json = JsonConvert.SerializeObject(Books, Formatting.Indented);
        File.WriteAllText(_currentFilePath, json);
    }

    public void AddBook(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book), "Книга не может быть null");

        book.Validate();

        if (Books.Any(b => b.ISBN == book.ISBN))
            throw new ArgumentException($"Книга с ISBN {book.ISBN} уже существует");

        Books.Add(book);
    }

    public void UpdateBook(int index, Book updatedBook)
    {
        if (index < 0 || index >= Books.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        updatedBook.Validate();
        Books[index] = updatedBook;
    }

    public void RemoveBook(int index)
    {
        if (index < 0 || index >= Books.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        Books.RemoveAt(index);
    }

    public void ImportFromCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
        var importedBooks = csv.GetRecords<Book>().ToList();
        
        foreach (var book in importedBooks)
        {
            book.Validate();
            if (Books.All(b => b.ISBN != book.ISBN))
            {
                Books.Add(book);
            }
        }
    }

    public void ExportToCsv(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(Books);
    }
}
