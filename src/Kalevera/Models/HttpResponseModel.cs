namespace Kalevera.Models;

public class HttpResponseModel
{
    public int StatusCode { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<RequestHeader> Headers { get; set; } = new();
    public long ElapsedMs { get; set; }
    public long ContentLength { get; set; }
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }
}
