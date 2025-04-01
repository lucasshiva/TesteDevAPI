namespace MinhaApiComSQLite.Extensions;

public static class StringExtensions
{
    public static string CapitalizeFirstLetter(this string value)
    {
        if (char.IsUpper(value[0]))
            return value;
        return char.ToUpper(value[0]) + value[1..];
    }
}
