using Refit;
using TkachevProject4.Models;

namespace TkachevProject4.Services.Interfaces;

public interface IOpenLibraryClient
{
    [Get("/api/books?bibkeys=ISBN:{isbn}&jscmd=data&format=json")]
    Task<Dictionary<string, OpenLibraryResponse>> GetBookInfo(string isbn);
}