using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Kalevera.Models;
using Newtonsoft.Json.Linq;

namespace Kalevera.Services;

public class HttpClientService
{
    private static readonly HttpClient _client = new();

    public async Task<HttpResponseModel> SendRequestAsync(
        HttpRequestModel request,
        Dictionary<string, string> environmentVars)
    {
        var response = new HttpResponseModel();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var method = request.Method.ToString();
            var httpMethod = new System.Net.Http.HttpMethod(method);
            var httpRequest = new HttpRequestMessage(httpMethod, request.Url);

            foreach (var header in request.Headers.Where(h => h.IsEnabled && !string.IsNullOrEmpty(h.Key)))
            {
                try
                {
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                catch { }
            }

            ApplyAuth(request, httpRequest);

            if (request.Method != HttpMethodType.GET && request.Method != HttpMethodType.DELETE &&
                request.BodyType != BodyType.None && !string.IsNullOrEmpty(request.BodyContent))
            {
                var body = EnvironmentService.ResolveVariables(request.BodyContent, environmentVars);
                var contentType = request.BodyType switch
                {
                    BodyType.Json => "application/json",
                    BodyType.Form => "application/x-www-form-urlencoded",
                    _ => "text/plain"
                };
                httpRequest.Content = new StringContent(body, Encoding.UTF8, contentType);
            }

            var httpResponse = await _client.SendAsync(httpRequest);
            stopwatch.Stop();

            response.StatusCode = (int)httpResponse.StatusCode;
            response.StatusText = httpResponse.ReasonPhrase ?? string.Empty;
            response.IsSuccess = httpResponse.IsSuccessStatusCode;
            response.ElapsedMs = stopwatch.ElapsedMilliseconds;

            var responseBody = await httpResponse.Content.ReadAsStringAsync();
            response.ContentLength = Encoding.UTF8.GetByteCount(responseBody);

            response.Body = FormatResponseBody(responseBody, httpResponse.Content.Headers.ContentType?.MediaType);

            foreach (var header in httpResponse.Headers)
            {
                foreach (var value in header.Value)
                {
                    response.Headers.Add(new RequestHeader { Key = header.Key, Value = value });
                }
            }
            foreach (var header in httpResponse.Content.Headers)
            {
                foreach (var value in header.Value)
                {
                    response.Headers.Add(new RequestHeader { Key = header.Key, Value = value });
                }
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            response.Error = ex.Message;
            response.IsSuccess = false;
            response.ElapsedMs = stopwatch.ElapsedMilliseconds;
        }

        return response;
    }

    private static void ApplyAuth(HttpRequestModel request, HttpRequestMessage httpRequest)
    {
        switch (request.AuthType)
        {
            case AuthType.Basic:
                var credentials = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{request.AuthUsername}:{request.AuthPassword}"));
                httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Basic {credentials}");
                break;
            case AuthType.Bearer:
                httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {request.AuthToken}");
                break;
        }
    }

    private static string FormatResponseBody(string body, string? contentType)
    {
        if (contentType != null && contentType.Contains("json"))
        {
            try
            {
                var parsed = JToken.Parse(body);
                return parsed.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch { }
        }
        return body;
    }
}
