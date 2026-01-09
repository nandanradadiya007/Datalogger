using PressureDataLogger.Interfaces;
using PressureDataLogger.Models;
using System.IO;
using System.Threading;

namespace PressureDataLogger.Services
{
    /// <summary>
    /// CSV logging service with async, non-blocking writes
    /// </summary>
    public class CSVLogger : ICSVLogger, IDisposable
    {
        private StreamWriter? _writer;
        private readonly SemaphoreSlim _writeLock = new(1, 1);

        public bool IsLogging { get; private set; }
        public string? LogFilePath { get; private set; }

        /// <summary>
        /// Starts logging to the specified CSV file
        /// </summary>
        public void StartLogging(string filePath)
        {
            if (IsLogging)
            {
                StopLogging();
            }

            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Open file for append
                _writer = new StreamWriter(filePath, append: true);
                _writer.AutoFlush = false; // Manual flush for better performance
                LogFilePath = filePath;
                
                // Write header if file is new/empty
                if (new FileInfo(filePath).Length == 0)
                {
                    _writer.WriteLine("Timestamp,Value");
                    _writer.Flush();
                }

                IsLogging = true;
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to start logging: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Stops logging and closes the file
        /// </summary>
        public void StopLogging()
        {
            if (!IsLogging) return;

            IsLogging = false;

            _writer?.Flush();
            _writer?.Close();
            _writer?.Dispose();
            _writer = null;
        }

        /// <summary>
        /// Logs a sample asynchronously
        /// </summary>
        public async Task LogSampleAsync(PressureSample sample)
        {
            if (!IsLogging || _writer == null) return;

            await _writeLock.WaitAsync();
            try
            {
                await _writer.WriteLineAsync($"{sample.Timestamp:yyyy-MM-dd HH:mm:ss.fff},{sample.Value:F6}");
                
                // Flush periodically for data safety
                if (_writer.BaseStream.Position % 4096 == 0)
                {
                    await _writer.FlushAsync();
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public void Dispose()
        {
            StopLogging();
            _writeLock.Dispose();
        }
    }
}
