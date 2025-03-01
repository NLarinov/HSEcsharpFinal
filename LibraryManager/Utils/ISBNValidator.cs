namespace TkachevProject4.Utils;

// ReSharper disable InconsistentNaming
public static class ISBNValidator
{
    public static bool IsValid(string? isbn)
    {
        isbn = isbn?.Replace("-", "").Trim();
            
        return (IsValidIsbn10(isbn) || IsValidIsbn13(isbn));
    }

    private static bool IsValidIsbn10(string? isbn)
    {
        if (isbn?.Length != 10) return false;

        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            char c = isbn[i];
            if (i == 9 && c == 'X')
                sum += 10;
            else if (!char.IsDigit(c))
                return false;
            else
                sum += (c - '0') * (10 - i);
        }
            
        return sum % 11 == 0;
    }

    private static bool IsValidIsbn13(string? isbn)
    {
        if (isbn?.Length != 13 || !isbn.All(char.IsDigit)) 
            return false;

        int sum = 0;

        for (int i = 0; i < 12; i++)
        {
            int digit = isbn[i] - '0';
            sum += digit * (i % 2 == 0 ? 1 : 3);
        }

        int checkDigit = (10 - sum % 10) % 10;

        return checkDigit == isbn[12] - '0';
    }
}