using System;

namespace GasMon.Models
{
    public class Notification
    {
        public string locationId { get; set; }
        public string eventId { get; set; }
        public double value { get; set; }
        public long timestamp { get; set; }

        public DateTime DateTime
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
            set => DateTime = value;
        }

        public override string ToString()
        {
            return $"\tEventId: {eventId}\n\tLocationId: {locationId}\n\tValue: {value}\n\tTimestamp: {DateTime}\n";
        }
    }
}