﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using CsvHelper;
using GasMon.Models;
using GasMon.S3Helper;
using GasMon.SQS;

namespace GasMon
{
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Getting locations...");
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

            try
            {
                var allNotifications = new List<Notification>();

                for (var i = 0; i < 5; i++)
                {
                    var sleepTime = 10;
                    Console.WriteLine($"Waiting for {sleepTime}s...");
                    Thread.Sleep(sleepTime * 1000);
                    Console.WriteLine("Getting notifications...");
                    
                    var notifications = await SQSService.GetNotificationFromQueue(queue, sqsClient);
                    var validNotifications = notifications
                        .Where(n => locationChecker.LocationIsValid(n))
                        .ToList();

                    allNotifications.AddRange(validNotifications);

                    var discarded = notifications.Where(n => !locationChecker.LocationIsValid(n));
                    Console.WriteLine($"Discarded {discarded.Count()} notifications with bad sensor");
                    foreach (var notification in validNotifications)
                    {
                        Console.WriteLine($"Number:{validNotifications.IndexOf(notification) + 1}");
                        Console.WriteLine(notification.ToString());
                    }

                    // CsvManager.CreateCsv(allNotifications);

                    if (i == 0)
                    {
                        CsvManager.CreateCsv(validNotifications);
                    }
                    
                    else
                    {
                        CsvManager.AppendCsv(validNotifications);
                    }
                }
                
                var averagedNotifications = new List<AveragedNotification>();
                // in csv, create a row for each location for each minute (averaging the value)
                foreach (var notification in allNotifications)
                {
                    var found = averagedNotifications.Where(n =>
                        n.LocationId == notification.locationId && n.Minute == notification.DateTime);
                    if (!found.Any())
                    {
                        averagedNotifications.Add(new AveragedNotification
                        {
                            AverageValue = notification.value,
                            LocationId = notification.locationId,
                            Minute = notification.DateTime,
                        });
                    }
                    else
                    {
                        found.First().
                    }
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