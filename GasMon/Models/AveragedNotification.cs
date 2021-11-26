using System;

namespace GasMon.Models
{
    public class AveragedNotification
    {
        public string LocationId { get; set; }
        public double AverageValue { get; set; }
        public DateTime Minute { get; set; }
    }
}