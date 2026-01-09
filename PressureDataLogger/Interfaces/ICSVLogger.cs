using PressureDataLogger.Models;

namespace PressureDataLogger.Interfaces
{
    /// <summary>
    /// Interface for CSV logging service
    /// </summary>
    public interface ICSVLogger
    {
        /// <summary>
        /// Gets or sets whether logging is enabled
        /// </summary>
        bool IsLogging { get; }

        /// <summary>
        /// Gets the current log file path
        /// </summary>
        string? LogFilePath { get; }

        /// <summary>
        /// Starts logging to a CSV file
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        void StartLogging(string filePath);

        /// <summary>
        /// Stops logging
        /// </summary>
        void StopLogging();

        /// <summary>
        /// Logs a pressure sample
        /// </summary>
        /// <param name="sample">The sample to log</param>
        Task LogSampleAsync(PressureSample sample);
    }
}
