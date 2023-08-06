using System.Text.RegularExpressions;

namespace notissimus.toyparser.core.Extensions;

public static class StringExtensions
{
    private static readonly Regex NonNumsRegex = new Regex("[^0-9]+", RegexOptions.Compiled);

    public static string RemoveNonNumChars(this string str)
    {
        return NonNumsRegex.Replace(str, String.Empty);
    }
}