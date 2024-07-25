using System.Text.Encodings.Web;
using Ganss.Xss;
using Microsoft.AspNetCore.WebUtilities;

namespace SuggestioApi.Helpers.Sanitize;

public static class Sanitize
{
    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sanitiser = new HtmlSanitizer();
        return sanitiser.Sanitize(input);
    }

    public static string SanitizeQueryString(string queryString)
    {
        var sanitizedQueryString = QueryHelpers.AddQueryString("/path", QueryHelpers.ParseQuery(queryString));
        return sanitizedQueryString;
    }

    public static string SanitizeUrlParameter(string urlParameter)
    {
        var sanitizedUrlParameter = UrlEncoder.Default.Encode(urlParameter);
        return sanitizedUrlParameter;
    }
}