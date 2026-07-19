namespace Kalevera.Models;

public class RequestHistoryEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public HttpRequestModel Request { get; set; } = new();
    public HttpResponseModel? Response { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
