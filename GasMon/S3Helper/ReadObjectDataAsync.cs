using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace GasMon.S3Helper
{
    public class S3Helper
    {
        private static readonly string BucketName = Environment.GetEnvironmentVariable("S3Bucket");
        private static readonly string KeyName = "locations.json";
        private static readonly RegionEndpoint BucketRegion = RegionEndpoint.EUWest1;
        private static IAmazonS3 _client;


        public static async Task ReadObjectDataAsync()
        {
            _client = new AmazonS3Client(BucketRegion);
            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = BucketName,
                    Key = KeyName
                };
                using (GetObjectResponse response = await _client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    string contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);

                    responseBody = reader.ReadToEnd(); // Now you process the response body.
                    Console.WriteLine(responseBody);
                }
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }
        }
    }
}