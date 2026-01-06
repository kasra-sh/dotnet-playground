namespace Boiler.Web.Host.AutoApi;

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]


public static class StringExtensions
{
    /// <summary>
    /// Converts a string (typically PascalCase or camelCase) to kebab-case (lowercase with hyphens).
    /// This implementation is highly optimized:
    /// - Exact allocation (no resizing or wasted space)
    /// - Minimal object allocations (only the final string and one char array)
    /// - Handles common cases and acronyms intelligently (e.g., "HTMLParser" → "htmlparser", "UserID" → "user-id", "GetHTMLContent" → "get-htmlcontent")
    /// </summary>
    public static string ToKebabCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // First pass: count how many hyphens will be inserted
        int hyphenCount = 0;
        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) &&
                (char.IsLower(input[i - 1]) || char.IsDigit(input[i - 1])))
            {
                hyphenCount++;
            }
        }

        // Allocate exact buffer size
        char[] buffer = new char[input.Length + hyphenCount];
        int index = 0;

        // First character (always lowered, no hyphen before it)
        buffer[index++] = char.ToLowerInvariant(input[0]);

        // Second pass: fill the buffer
        for (int i = 1; i < input.Length; i++)
        {
            char current = input[i];

            // Insert hyphen only on transition from lower/digit to upper
            if (char.IsUpper(current) &&
                (char.IsLower(input[i - 1]) || char.IsDigit(input[i - 1])))
            {
                buffer[index++] = '-';
            }

            buffer[index++] = char.ToLowerInvariant(current);
        }

        return new string(buffer);
    }
}