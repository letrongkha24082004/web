using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ShopManager.Extensions;

public static partial class SlugHelper
{
    public static string ToSlug(this string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        var text = builder.ToString().Normalize(NormalizationForm.FormC).Replace('đ', 'd');
        return MultipleDashes().Replace(InvalidCharacters().Replace(text, "-"), "-").Trim('-');
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex InvalidCharacters();

    [GeneratedRegex("-{2,}")]
    private static partial Regex MultipleDashes();
}
