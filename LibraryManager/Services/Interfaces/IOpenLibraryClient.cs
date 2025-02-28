using Refit;
using TkachevProject4.Models;

namespace TkachevProject4.Services.Interfaces;

public interface IOpenLibraryClient
{
    [Get("/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data")]
    Task<Dictionary<string, OpenLibraryResponse>> GetBookInfo(string isbn);
}