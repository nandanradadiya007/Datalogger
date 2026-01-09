namespace PressureDataLogger.Models
{
    /// <summary>
    /// Represents a single temperature data sample from a thermocouple
    /// </summary>
    public class TemperatureSample
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; } // Temperature in Celsius
        public string ChannelName { get; set; }

        public TemperatureSample(DateTime timestamp, double value, string channelName = "TC1")
        {
            Timestamp = timestamp;
            Value = value;
            ChannelName = channelName;
        }

        public double ValueFahrenheit => (Value * 9.0 / 5.0) + 32.0;
        public double ValueKelvin => Value + 273.15;

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff}, {ChannelName}, {Value:F2}°C";
        }
    }
}
