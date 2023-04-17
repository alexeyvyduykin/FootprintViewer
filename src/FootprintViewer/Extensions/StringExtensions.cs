namespace FootprintViewer.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        if (value.Length < 2)
        {
            return value.ToUpper();
        }

        return char.ToUpper(value[0]) + value[1..];
    }

    public static string TrimEnd(this string me, string trimString, StringComparison comparisonType)
    {
        if (me.EndsWith(trimString, comparisonType))
        {
            return me[..^trimString.Length];
        }
        return me;
    }
}
