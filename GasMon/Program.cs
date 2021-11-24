using System;
using System.Linq;
using System.Threading.Tasks;
using static GasMon.S3Helper.S3Helper;

namespace GasMon
{
    class Program
    {

        public static async Task Main()
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"));
            Console.WriteLine(Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"));
            var locations = await ReadObjectDataAsync();
            foreach (var location in locations)
            {
                Console.WriteLine($"Id: {location.id}, Long: {location.x}, Lat: {location.y}");
            }
        }
        
    }
}