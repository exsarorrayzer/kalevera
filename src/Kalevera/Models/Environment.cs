namespace Kalevera.Models;

public class Environment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "New Environment";
    public List<EnvironmentVariable> Variables { get; set; } = new();
}
