namespace Kalevera.Models;

public class Collection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "New Collection";
    public List<HttpRequestModel> Requests { get; set; } = new();
}
