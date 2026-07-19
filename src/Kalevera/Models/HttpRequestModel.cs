namespace Kalevera.Models;

public class HttpRequestModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "New Request";
    public HttpMethodType Method { get; set; } = HttpMethodType.GET;
    public string Url { get; set; } = string.Empty;
    public List<RequestParameter> Parameters { get; set; } = new();
    public List<RequestHeader> Headers { get; set; } = new();
    public BodyType BodyType { get; set; } = BodyType.None;
    public string BodyContent { get; set; } = string.Empty;
    public AuthType AuthType { get; set; } = AuthType.None;
    public string AuthUsername { get; set; } = string.Empty;
    public string AuthPassword { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
}

public enum BodyType
{
    None,
    Json,
    Form,
    Raw
}
