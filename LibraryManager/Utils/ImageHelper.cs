using SkiaSharp;
using Spectre.Console;

namespace TkachevProject4.Utils;

public static class ImageHelper
{
    public static void DisplayBookCover(string? imagePath)
    {
        if (!File.Exists(imagePath)) return;

        try
        {
            using var image = SKBitmap.Decode(imagePath);
            var canvas = new Canvas(image.Width, image.Height);
                
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var color = image.GetPixel(x, y);
                    canvas.SetPixel(x, y, new Color(color.Red, color.Green, color.Blue));
                }
            }
                
            AnsiConsole.Write(canvas);
        }
        catch
        {
            AnsiConsole.MarkupLine("[red]Ошибка загрузки обложки[/]");
        }
    }
}