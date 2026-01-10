namespace PressureDataLogger.Models
{
    /// <summary>
    /// Represents calibration data for a sensor
    /// </summary>
    public class CalibrationData
    {
        public string SensorId { get; set; }
        public string SensorType { get; set; } // "Pressure", "Temperature"
        public double Offset { get; set; }
        public double Gain { get; set; }
        public DateTime CalibrationDate { get; set; }
        public string CalibrationNotes { get; set; }
        public List<CalibrationPoint> CalibrationPoints { get; set; }

        public CalibrationData(string sensorId, string sensorType)
        {
            SensorId = sensorId;
            SensorType = sensorType;
            Offset = 0.0;
            Gain = 1.0;
            CalibrationDate = DateTime.Now;
            CalibrationNotes = "";
            CalibrationPoints = new List<CalibrationPoint>();
        }

        /// <summary>
        /// Apply calibration to a raw value
        /// </summary>
        public double ApplyCalibration(double rawValue)
        {
            return (rawValue * Gain) + Offset;
        }

        public override string ToString()
        {
            return $"{SensorType} - {SensorId}: Gain={Gain:F4}, Offset={Offset:F4} (Cal: {CalibrationDate:yyyy-MM-dd})";
        }
    }

    /// <summary>
    /// Represents a single calibration point
    /// </summary>
    public class CalibrationPoint
    {
        public double ReferenceValue { get; set; }
        public double MeasuredValue { get; set; }

        public CalibrationPoint(double referenceValue, double measuredValue)
        {
            ReferenceValue = referenceValue;
            MeasuredValue = measuredValue;
        }
    }
}
