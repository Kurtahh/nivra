using backend.Enums;
using backend.Exceptions;

namespace backend.Extensions;

public static class StringExtensions
{
    public static TipCategory ToTipCategory(this string text)
    {
        if (Enum.TryParse<TipCategory>(text, ignoreCase: true, out var category)
            && Enum.IsDefined(category))
        {
            return category;
        }

        throw new InvalidTipCategoryException(text, Enum.GetNames<TipCategory>());
    }
}