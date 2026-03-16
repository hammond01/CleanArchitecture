namespace DbMigrator;

/// <summary>
/// Migration configuration settings
/// </summary>
public class MigrationSettings
{
    /// <summary>
    /// Whether to seed initial data after migrations
    /// </summary>
    public bool SeedData { get; set; } = true;

    /// <summary>
    /// Whether to create database if it doesn't exist
    /// </summary>
    public bool CreateDatabaseIfNotExists { get; set; } = true;

    /// <summary>
    /// Migration timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 300;
}
