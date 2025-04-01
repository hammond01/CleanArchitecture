namespace ProductManager.Shared.Logging;

/// <summary>
/// Configuration options for log file rotation and compression.
/// </summary>
public class LogRotationOptions
{
    /// <summary>
    /// Maximum size of a single log file in bytes before rotation occurs.
    /// Default value is 10MB (10 * 1024 * 1024 bytes).
    /// </summary>
    public long MaxFileSizeInBytes { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// Maximum number of rotated log files to keep.
    /// When this limit is exceeded, the oldest files will be deleted.
    /// Default value is 5.
    /// </summary>
    public int MaxFileCount { get; set; } = 5;

    /// <summary>
    /// Pattern for naming rotated log files.
    /// {0} represents the base filename, {1} represents the date.
    /// Default value is "{0}.{1}".
    /// </summary>
    public string FileNamePattern { get; set; } = "{0}.{1}";

    /// <summary>
    /// Date format string used in rotated log file names.
    /// Default value is "yyyyMMdd".
    /// </summary>
    public string DateFormat { get; set; } = "yyyyMMdd";

    /// <summary>
    /// Whether to compress rotated log files.
    /// If enabled, rotated files will be compressed using GZip compression.
    /// Default value is true.
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// File extension for compressed log files.
    /// Default value is ".gz".
    /// </summary>
    public string CompressedFileExtension { get; set; } = ".gz";

    /// <summary>
    /// Compression level for GZip compression (0-9).
    /// Higher values provide better compression but are slower.
    /// Default value is 6.
    /// </summary>
    public int CompressionLevel { get; set; } = 6;

    /// <summary>
    /// Gets the default configuration options.
    /// </summary>
    public static LogRotationOptions Default => new();
}
