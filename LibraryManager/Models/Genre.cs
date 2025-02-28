namespace TkachevProject4.Models;

using System.ComponentModel;

public enum Genre
{
    [Description("Фантастика")]
    Fantasy,
    [Description("Детектив")]
    Detective,
    [Description("Роман")]
    Novel,
    [Description("История")]
    History,
    [Description("Научная литература")]
    Science,
    [Description("Другое")]
    Other
}
