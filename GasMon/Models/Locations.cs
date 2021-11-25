namespace GasMon.Models
{
    public class Location
    {
        public double x { get; set; }
        public double y { get; set; }
        public string id { get; set; }

        public override string ToString()
        {
            return $"Id:{id}, Long: {x}, Lat: {y}";
        }
    }
}