using TkachevProject4.Models;

namespace TkachevProject4.Services.Interfaces;

public interface ILibraryService
{
    List<Book> Books { get; }
    void LoadLibrary(string filePath);
    void SaveLibrary();
    void AddBook(Book book);
    void UpdateBook(int index, Book updatedBook);
    void RemoveBook(int index);
    void ImportFromCsv(string filePath);
    void ExportToCsv(string filePath);
}