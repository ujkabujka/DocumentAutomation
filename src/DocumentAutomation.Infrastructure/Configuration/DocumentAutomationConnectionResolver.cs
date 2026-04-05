using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DocumentAutomation.Infrastructure.Configuration;

public static class DocumentAutomationConnectionResolver
{
    public static DocumentAutomationConnectionResolution Resolve(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DocumentAutomation")
            ?? Environment.GetEnvironmentVariable("DOCUMENT_AUTOMATION_CONNECTION_STRING")
            ?? Environment.GetEnvironmentVariable("PG_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return new DocumentAutomationConnectionResolution(null, "Database connection not configured.", false);
        }

        connectionString = connectionString.Trim();

        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            return new DocumentAutomationConnectionResolution(
                connectionString,
                $"Connected to {builder.Database} at {builder.Host}:{builder.Port}.",
                true);
        }
        catch (Exception ex)
        {
            return new DocumentAutomationConnectionResolution(
                connectionString,
                $"Database unavailable: {ex.Message}",
                false);
        }
    }

    public static DocumentAutomationStorageSettings ResolveStorage(IConfiguration configuration)
    {
        var templateRoot = configuration["Storage:TemplateRoot"]
            ?? Path.Combine(AppContext.BaseDirectory, "data", "templates");
        var outputRoot = configuration["Storage:OutputRoot"]
            ?? Path.Combine(AppContext.BaseDirectory, "data", "output");

        return new DocumentAutomationStorageSettings(
            Path.GetFullPath(templateRoot),
            Path.GetFullPath(outputRoot));
    }
}
