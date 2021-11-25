using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using GasMon.S3Helper;
using GasMon.SQS;

namespace GasMon
{
    class Program
    {
        public static async Task Main()
        {
            var locations = await S3Service.ReadObjectDataAsync();
            var sqsClient = new AmazonSQSClient(RegionEndpoint.EUWest1);
            var snsClient = new AmazonSimpleNotificationServiceClient(RegionEndpoint.EUWest1);
            var locationChecker = new LocationChecker(locations);

            // Create queue
            var queue = await SQSService.CreateQueue(sqsClient);
            var topicArn = Environment.GetEnvironmentVariable("SNSTopic");

            //Subscribe Queue to SNS topic
            await snsClient.SubscribeQueueAsync(topicArn, sqsClient, queue.QueueUrl);

            Console.WriteLine("Locations: ");
            foreach (var location in locations)
            {
                Console.WriteLine(location.ToString());
            }

            Console.WriteLine("Waiting for 20s...");
            DateTime now = DateTime.Now;
            while (DateTime.Now.Subtract(now).Seconds < 60)
            {
                // wait for 20 seconds
            }

            try
            {
                var notifications = await SQSService.GetNotificationFromQueue(queue, sqsClient);
                var validNotifications = notifications.Where(n => locationChecker.LocationIsValid(n));
                var discarded = notifications.Where(n => !locationChecker.LocationIsValid(n));
                Console.WriteLine($"Discarded {discarded.Count()} notifications with bad sensors");
                foreach (var notification in validNotifications)
                {
                    Console.WriteLine($"Number:{notifications.IndexOf(notification) + 1}");
                    Console.WriteLine(notification.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                await SQSService.DeleteQueue(sqsClient, queue);
            }
        }
    }
}