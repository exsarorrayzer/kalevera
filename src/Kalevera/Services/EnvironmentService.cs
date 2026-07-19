using System.Text.RegularExpressions;
using Kalevera.Models;
using Environment = Kalevera.Models.Environment;

namespace Kalevera.Services;

public static partial class EnvironmentService
{
    public static string ResolveVariables(string input, Dictionary<string, string> variables)
    {
        if (string.IsNullOrEmpty(input) || variables.Count == 0)
            return input;

        return VariablePattern().Replace(input, match =>
        {
            var key = match.Groups[1].Value.Trim();
            return variables.TryGetValue(key, out var value) ? value : match.Value;
        });
    }

    public static Dictionary<string, string> ToDictionary(Environment environment)
    {
        return environment.Variables
            .Where(v => v.IsEnabled && !string.IsNullOrEmpty(v.Key))
            .ToDictionary(v => v.Key, v => v.Value);
    }

    [GeneratedRegex(@"\{\{(.+?)\}\}")]
    private static partial Regex VariablePattern();
}
