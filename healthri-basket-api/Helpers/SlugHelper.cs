using Slugify;

namespace healthri_basket_api.Helpers;

public static class SlugHelper
{
    private static readonly Slugify.SlugHelper Helper = new();

    public static string Slugify(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return Helper.GenerateSlug(text);
    }
}
