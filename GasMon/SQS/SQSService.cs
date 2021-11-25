using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using GasMon.Models;

namespace GasMon.SQS
{
    public class SQSService
    {
        // <summary>
        /// Initializes the Amazon SQS client object and then calls the
        /// CreateQueueAsync method to create the new queue. If the call is
        /// successful, it displays the URL of the new queue on the console.
        /// </summary>
        public static async Task<CreateQueueResponse> CreateQueue(AmazonSQSClient client)
        {
            // If the Amazon SQS message queue is not in the same AWS Region as your
            // default user, you need to provide the AWS Region as a parameter to the
            // client constructor.

            string queueName = "Jordan-Gasmon-Queue";
            int maxMessage = 262144;
            var attrs = new Dictionary<string, string>
            {
                {
                    QueueAttributeName.DelaySeconds,
                    TimeSpan.FromSeconds(5).TotalSeconds.ToString()
                },
                {
                    QueueAttributeName.MaximumMessageSize,
                    maxMessage.ToString()
                },
                {
                    QueueAttributeName.MessageRetentionPeriod,
                    TimeSpan.FromDays(4).TotalSeconds.ToString()
                },
                {
                    QueueAttributeName.ReceiveMessageWaitTimeSeconds,
                    TimeSpan.FromSeconds(20).TotalSeconds.ToString()
                },
                {
                    QueueAttributeName.VisibilityTimeout,
                    TimeSpan.FromHours(12).TotalSeconds.ToString()
                },
            };

            var request = new CreateQueueRequest
            {
                Attributes = attrs,
                QueueName = queueName,
            };

            var response = await client.CreateQueueAsync(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Successfully created Amazon SQS queue.");
                Console.WriteLine($"Queue URL: {response.QueueUrl}");
            }

            return response;
        }

        public static async Task DeleteQueue(AmazonSQSClient client, CreateQueueResponse queueResponse)
        {
            // If the Amazon SQS message queue is not in the same AWS Region as your
            // default user, you need to provide the AWS Region as a parameter to the
            // client constructor.

            string queueUrl = queueResponse.QueueUrl;

            var response = await client.DeleteQueueAsync(queueUrl);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Successfully deleted the queue.");
            }
            else
            {
                Console.WriteLine("Could not delete the queue.");
            }
        }

        public static async Task<string> GetQueueArn(IAmazonSQS sqsClient, string qUrl)
        {
            GetQueueAttributesResponse responseGetAtt = await sqsClient.GetQueueAttributesAsync(
                qUrl, new List<string> {QueueAttributeName.QueueArn});
            return responseGetAtt.QueueARN;
        }

        public static void DisplayMessages(List<Message> messages)
        {
            messages.ForEach(m =>
            {
                Console.WriteLine($"For message ID {m.MessageId}:");
                Console.WriteLine($"  Body: {m.Body}");
                Console.WriteLine($"  Receipt handle: {m.ReceiptHandle}");
                Console.WriteLine($"  MD5 of body: {m.MD5OfBody}");
                Console.WriteLine($"  MD5 of message attributes: {m.MD5OfMessageAttributes}");
                Console.WriteLine("  Attributes:");

                foreach (var attr in m.Attributes)
                {
                    Console.WriteLine($"\t {attr.Key}: {attr.Value}");
                }
            });
        }

        public static List<Notification> DeserializeMessages(List<Message> messages)
        {
            var convertedMessages = new List<Notification>();
            messages.ForEach(m =>
            {
                var converted = JsonSerializer.Deserialize<Body>(m.Body);
                var unescaped = Regex.Unescape(converted.Message);
                var bodyMessage = JsonSerializer.Deserialize<Notification>(unescaped);
                convertedMessages.Add(bodyMessage);
            });
            return convertedMessages;
        }

        public static async Task<List<Notification>> GetNotificationFromQueue(CreateQueueResponse queue,
            AmazonSQSClient sqsClient)
        {
            var notifications = new List<Notification>();
            string queueUrl = queue.QueueUrl;
            var attributeNames = new List<string>() {"All"};
            int maxNumberOfMessages = 10;
            var visibilityTimeout = (int) TimeSpan.FromMinutes(10).TotalSeconds;
            var waitTimeSeconds = (int) TimeSpan.FromSeconds(20).TotalSeconds;


            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                AttributeNames = attributeNames,
                MaxNumberOfMessages = maxNumberOfMessages,
                VisibilityTimeout = visibilityTimeout,
                WaitTimeSeconds = waitTimeSeconds,
            };

            var response = await sqsClient.ReceiveMessageAsync(request);

            if (response.Messages.Count > 0)
            {
                // SQSService.DisplayMessages(response.Messages);
                notifications = DeserializeMessages(response.Messages);
            }

            return notifications;
        }
    }
}