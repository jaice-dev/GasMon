namespace GasMon.Models
{
    // [System.Serializable]
    // public static class Locations
    // {
    //    public const string One = "5c3130a0-3a6f-4f5b-929d-ff40d606e056";
    //    public const string Two = "34441994-1b00-4cc4-9f66-7f071166a41b";
    //    public const string Three = "e64bd116-599c-4e70-b721-686aa393db8b";
    //    public const string Four = "74bf5f65-9c5d-4fc3-98a6-baab43b9628f";
    //    public const string Five = "e0fb522b-9289-446c-b712-7cccac8a7df7";
    //    public const string Six = "c096f7d4-57d4-4f0c-82e4-15c1c012b1a8";
    //    public const string Seven = "53307446-9a3a-4079-bfaf-e8901117b6f4";
    //    public const string Eight = "abe107bd-c1d3-43dd-a017-60285a8ca80a";
    //    public const string Nine = "bf859fd8-d01e-4ea5-9d2c-8c1a6abb246b";
    //    public const string Ten = "7a2411a1-04e8-4dcb-a989-e1ecbac16617";
    //
    // }


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