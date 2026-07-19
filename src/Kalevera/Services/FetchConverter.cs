using System.Text.RegularExpressions;
using System.Text;
using Kalevera.Models;

namespace Kalevera.Services;

public static partial class FetchConverter
{
    public static HttpRequestModel Parse(string fetchText)
    {
        var text = fetchText.Trim();

        text = StripComments(text);

        var request = new HttpRequestModel();

        var urlMatch = FetchUrlPattern().Match(text);
        if (!urlMatch.Success)
            throw new Exception("Could not find a URL in the fetch command.");

        request.Url = urlMatch.Groups[1].Value.Trim().Trim('\'', '"');

        var methodMatch = FetchMethodPattern().Match(text);
        if (methodMatch.Success)
        {
            var method = methodMatch.Groups[1].Value.Trim().Trim('\'', '"').ToUpper();
            if (Enum.TryParse<HttpMethodType>(method, out var m))
                request.Method = m;
        }
        else
        {
            request.Method = HttpMethodType.GET;
        }

        ParseHeaders(text, request);
        ParseBody(text, request);

        request.Name = $"{request.Method} {ShortenUrl(request.Url)}";

        return request;
    }

    private static string StripComments(string text)
    {
        return SingleLineCommentPattern().Replace(text, "");
    }

    private static void ParseHeaders(string text, HttpRequestModel request)
    {
        var headersMatch = HeadersPattern().Match(text);
        if (!headersMatch.Success) return;

        var headersBlock = headersMatch.Groups[1].Value;
        var pairs = HeaderPairPattern().Matches(headersBlock);

        foreach (Match pair in pairs)
        {
            var key = pair.Groups[1].Value.Trim().Trim('\'', '"');
            var value = pair.Groups[2].Value.Trim().Trim('\'', '"');
            if (!string.IsNullOrEmpty(key))
                request.Headers.Add(new RequestHeader { Key = key, Value = value });
        }
    }

    private static void ParseBody(string text, HttpRequestModel request)
    {
        var bodyMatch = BodyPattern().Match(text);
        if (!bodyMatch.Success) return;

        var bodyContent = bodyMatch.Groups[1].Value.Trim();

        bodyContent = CleanStringConcat(bodyContent);

        if (IsJson(bodyContent))
        {
            request.BodyType = BodyType.Json;
            request.BodyContent = FormatJson(bodyContent);
        }
        else if (bodyContent.Contains("=") && !bodyContent.StartsWith("{"))
        {
            request.BodyType = BodyType.Form;
            request.BodyContent = bodyContent;
        }
        else
        {
            request.BodyType = BodyType.Raw;
            request.BodyContent = bodyContent.Trim('\'', '"');
        }
    }

    private static string CleanStringConcat(string body)
    {
        return StringConcatPattern().Replace(body, match =>
        {
            var parts = new List<string>();
            foreach (Capture c in match.Groups[1].Captures)
                parts.Add(c.Value.Trim().Trim('\'', '"'));
            return string.Concat(parts);
        });
    }

    private static bool IsJson(string text)
    {
        var trimmed = text.Trim();
        return (trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
               (trimmed.StartsWith("[") && trimmed.EndsWith("]"));
    }

    private static string FormatJson(string text)
    {
        try
        {
            var trimmed = text.Trim();
            var parsed = Newtonsoft.Json.Linq.JToken.Parse(trimmed);
            return parsed.ToString(Newtonsoft.Json.Formatting.Indented);
        }
        catch
        {
            return text;
        }
    }

    private static string ShortenUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath;
            if (path.Length > 40)
                path = path.Substring(0, 37) + "...";
            return $"{uri.Host}{path}";
        }
        catch
        {
            return url.Length > 50 ? url.Substring(0, 47) + "..." : url;
        }
    }

    [GeneratedRegex(@"fetch\(\s*['""]([^'""]+)['""]")]
    private static partial Regex FetchUrlPattern();

    [GeneratedRegex(@"method:\s*['""](\w+)['""]")]
    private static partial Regex FetchMethodPattern();

    [GeneratedRegex(@"headers:\s*\{([^}]*)\}", RegexOptions.Singleline)]
    private static partial Regex HeadersPattern();

    [GeneratedRegex(@"['""]?([\w\-]+)['""]?\s*:\s*['""]([^'""]*)['""]")]
    private static partial Regex HeaderPairPattern();

    [GeneratedRegex(@"body:\s*(\{[\s\S]*?\}(?:\s*\+\s*\{[\s\S]*?\})*|'[^']*'|""[^""]*"")")]
    private static partial Regex BodyPattern();

    [GeneratedRegex(@"'([^']*)'\s*\+\s*'([^']*)'")]
    private static partial Regex StringConcatPattern();

    [GeneratedRegex(@"//.*$", RegexOptions.Multiline)]
    private static partial Regex SingleLineCommentPattern();
}
