using System.IO.Compression;

namespace ProductManager.Shared.Logging;

/// <summary>
/// A file-based logger implementation that supports log rotation and compression.
/// This logger automatically rotates log files when they reach a specified size
/// and optionally compresses old log files to save disk space.
/// </summary>
public class FileLogger : LoggerBase
{
    private readonly string _logFilePath;
    private readonly object _lockObject = new();
    private readonly LogRotationOptions _rotationOptions;

    /// <summary>
    /// Initializes a new instance of the FileLogger class.
    /// </summary>
    /// <param name="logFilePath">The path to the log file.</param>
    /// <param name="minimumLevel">The minimum log level to log. Default is Information.</param>
    /// <param name="rotationOptions">Configuration options for log rotation and compression.</param>
    public FileLogger(string logFilePath, LogLevel minimumLevel = LogLevel.Information, LogRotationOptions? rotationOptions = null)
        : base(minimumLevel)
    {
        _logFilePath = logFilePath;
        _rotationOptions = rotationOptions ?? LogRotationOptions.Default;
        EnsureLogDirectoryExists();
    }

    /// <summary>
    /// Ensures that the directory for the log file exists.
    /// </summary>
    private void EnsureLogDirectoryExists()
    {
        var directory = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// Logs a message to the log file.
    /// </summary>
    /// <param name="entry">The log entry to write.</param>
    protected override void Log(LogEntry entry)
    {
        var logMessage = FormatLogMessage(entry);
        lock (_lockObject)
        {
            CheckAndRotateLogFile();
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
    }

    /// <summary>
    /// Checks if the current log file needs to be rotated based on its size.
    /// </summary>
    private void CheckAndRotateLogFile()
    {
        if (!File.Exists(_logFilePath))
            return;

        var fileInfo = new FileInfo(_logFilePath);
        if (fileInfo.Length < _rotationOptions.MaxFileSizeInBytes)
            return;

        RotateLogFile();
    }

    /// <summary>
    /// Rotates the current log file by renaming it with a date stamp
    /// and optionally compressing it.
    /// </summary>
    private void RotateLogFile()
    {
        var directory = Path.GetDirectoryName(_logFilePath);
        var fileName = Path.GetFileNameWithoutExtension(_logFilePath);
        var extension = Path.GetExtension(_logFilePath);
        var date = DateTime.Now.ToString(_rotationOptions.DateFormat);

        // Create new filename with date
        var newFileName = string.Format(_rotationOptions.FileNamePattern, fileName, date);
        var newFilePath = Path.Combine(directory!, newFileName + extension);

        // Move current file to new name
        File.Move(_logFilePath, newFilePath);

        // Compress file if enabled
        if (_rotationOptions.EnableCompression)
        {
            CompressFile(newFilePath);
        }

        // Clean up old files if exceeding max count
        CleanupOldFiles(directory!, fileName, extension);
    }

    /// <summary>
    /// Compresses a log file using GZip compression.
    /// </summary>
    /// <param name="filePath">The path to the file to compress.</param>
    private void CompressFile(string filePath)
    {
        try
        {
            var compressedFilePath = filePath + _rotationOptions.CompressedFileExtension;
            using var sourceStream = File.OpenRead(filePath);
            using var destinationStream = File.Create(compressedFilePath);
            using var gzipStream = new GZipStream(destinationStream, (CompressionLevel)_rotationOptions.CompressionLevel);
            sourceStream.CopyTo(gzipStream);

            // Delete original file after successful compression
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            base.LogError($"Failed to compress log file: {filePath}", ex);
        }
    }

    /// <summary>
    /// Cleans up old log files when the maximum file count is exceeded.
    /// </summary>
    /// <param name="directory">The directory containing log files.</param>
    /// <param name="fileName">The base name of the log files.</param>
    /// <param name="extension">The file extension.</param>
    private void CleanupOldFiles(string directory, string fileName, string extension)
    {
        var pattern = string.Format(_rotationOptions.FileNamePattern, fileName, "*") + extension;
        if (_rotationOptions.EnableCompression)
        {
            pattern += _rotationOptions.CompressedFileExtension;
        }

        var files = Directory.GetFiles(directory, pattern)
            .OrderByDescending(f => f)
            .ToList();

        // Delete oldest files if exceeding max count
        while (files.Count > _rotationOptions.MaxFileCount)
        {
            var oldestFile = files.Last();
            try
            {
                File.Delete(oldestFile);
                files.Remove(oldestFile);
            }
            catch (Exception ex)
            {
                base.LogError($"Failed to delete old log file: {oldestFile}", ex);
            }
        }
    }

    /// <summary>
    /// Formats a log entry into a string message.
    /// </summary>
    /// <param name="entry">The log entry to format.</param>
    /// <returns>A formatted string representation of the log entry.</returns>
    private string FormatLogMessage(LogEntry entry)
    {
        var message = $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{entry.Level}] {entry.Message}";

        if (entry.Exception != null)
        {
            message += $"\nException: {entry.Exception.Message}";
            message += $"\nStackTrace: {entry.Exception.StackTrace}";
        }

        if (entry.AdditionalData?.Any() == true)
        {
            message += "\nAdditional Data:";
            foreach (var data in entry.AdditionalData)
            {
                message += $"\n  {data.Key}: {data.Value}";
            }
        }

        return message;
    }
}
