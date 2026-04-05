using System.Text.Json;
using Microsoft.EntityFrameworkCore.Design;

namespace DocumentAutomation.Persistence.Configuration;

public sealed class DocumentAutomationDbContextFactory : IDesignTimeDbContextFactory<DocumentAutomationDbContext>
{
    public DocumentAutomationDbContext CreateDbContext(string[] args)
        => new(ResolveConnectionString());

    private static string ResolveConnectionString()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("DOCUMENT_AUTOMATION_CONNECTION_STRING")
            ?? Environment.GetEnvironmentVariable("PG_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment.Trim();
        }

        foreach (var path in CandidateFiles())
        {
            if (!File.Exists(path))
            {
                continue;
            }

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            if (document.RootElement.TryGetProperty("ConnectionStrings", out var section) &&
                section.TryGetProperty("DocumentAutomation", out var value) &&
                value.ValueKind == JsonValueKind.String)
            {
                return value.GetString()!.Trim();
            }
        }

        return "Host=localhost;Port=5432;Database=document_automation;Username=postgres;Password=devpassword";
    }

    private static IEnumerable<string> CandidateFiles()
    {
        var directories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddTree(Directory.GetCurrentDirectory(), directories);
        AddTree(AppContext.BaseDirectory, directories);

        foreach (var directory in directories)
        {
            yield return Path.Combine(directory, "appsettings.local.json");
            yield return Path.Combine(directory, "appsettings.json");
            yield return Path.Combine(directory, "src", "DocumentAutomation.App", "appsettings.local.json");
            yield return Path.Combine(directory, "src", "DocumentAutomation.App", "appsettings.json");
        }
    }

    private static void AddTree(string? directory, ISet<string> directories)
    {
        var current = directory;
        while (!string.IsNullOrWhiteSpace(current))
        {
            directories.Add(current);
            current = Directory.GetParent(current)?.FullName;
        }
    }
}
