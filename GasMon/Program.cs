using System;
using static GasMon.S3Helper.S3Helper;

namespace GasMon
{
    class Program
    {

        public static void Main()
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"));
            Console.WriteLine(Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"));
            ReadObjectDataAsync().Wait();
        }
        
    }
}