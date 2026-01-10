namespace PressureDataLogger.Models
{
    /// <summary>
    /// Represents the state of a valve
    /// </summary>
    public class ValveState
    {
        public string ValveId { get; set; }
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public DateTime LastChanged { get; set; }
        public string Description { get; set; }

        public ValveState(string valveId, string name, bool isOpen = false, string description = "")
        {
            ValveId = valveId;
            Name = name;
            IsOpen = isOpen;
            LastChanged = DateTime.Now;
            Description = description;
        }

        public string Status => IsOpen ? "OPEN" : "CLOSED";

        public override string ToString()
        {
            return $"{Name} ({ValveId}): {Status} - Last Changed: {LastChanged:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
