using PressureDataLogger.Models;

namespace PressureDataLogger.Interfaces
{
    /// <summary>
    /// Interface for data acquisition from NI-DAQmx devices
    /// </summary>
    public interface IDAQManager
    {
        /// <summary>
        /// Event raised when new data is acquired
        /// </summary>
        event EventHandler<PressureSample>? DataAcquired;

        /// <summary>
        /// Event raised when an error occurs
        /// </summary>
        event EventHandler<string>? ErrorOccurred;

        /// <summary>
        /// Gets whether acquisition is currently running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts data acquisition
        /// </summary>
        /// <param name="deviceChannel">Device and channel (e.g., Dev1/ai0)</param>
        /// <param name="sampleRate">Sample rate in Hz</param>
        /// <param name="samplesPerRead">Number of samples to read per iteration</param>
        void Start(string deviceChannel, int sampleRate, int samplesPerRead);

        /// <summary>
        /// Stops data acquisition
        /// </summary>
        void Stop();
    }
}
