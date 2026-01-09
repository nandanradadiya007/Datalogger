namespace PressureDataLogger.Models
{
    /// <summary>
    /// Represents a single pressure data sample with timestamp
    /// </summary>
    public class PressureSample
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }

        public PressureSample(DateTime timestamp, double value)
        {
            Timestamp = timestamp;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff}, {Value:F6}";
        }
    }
}
