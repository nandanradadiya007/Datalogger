namespace PressureDataLogger.Models
{
    /// <summary>
    /// Application configuration with default values
    /// </summary>
    public class AppConfiguration
    {
        // Default NI-DAQmx configuration
        public const string DefaultDeviceChannel = "Dev1/ai0";
        public const int DefaultSampleRate = 1000; // Hz
        public const int DefaultSamplesPerRead = 100;
        
        // Voltage range for pressure transducers (typical)
        public const double MinVoltage = -10.0;
        public const double MaxVoltage = 10.0;
        
        // CSV configuration
        public const string DefaultLogFileName = "PressureData.csv";
        public const int MaxDisplayedSamples = 100;
    }
}
