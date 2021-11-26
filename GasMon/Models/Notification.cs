using System;


namespace GasMon.Models
{
    public class Notification
    {
        public static DateTime RoundDown(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks / d.Ticks) * d.Ticks);
        }
        
        public string locationId { get; set; }
        public string eventId { get; set; }
        public double value { get; set; }
        public long timestamp { get; set; }

        
        public DateTime DateTime => RoundDown(DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime, TimeSpan.FromMinutes(1));

        public override string ToString()
        {
            return $"\tEventId: {eventId}\n\tLocationId: {locationId}\n\tValue: {value}\n\tTimestamp: {DateTime}\n";
        }
    }
}