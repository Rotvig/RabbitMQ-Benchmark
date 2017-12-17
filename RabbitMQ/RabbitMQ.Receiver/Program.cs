using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Receiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var ip = "localhost";
            var expectedNumberOfmessages = 10000;
            if (args.Length > 0)
            {
                ip = args.First();
                expectedNumberOfmessages = Int32.Parse(args.Last());
            }
            Console.WriteLine("Running with IP: " + ip);
            var factory = new ConnectionFactory {HostName = ip, UserName = "test", Password = "test"};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("logs", "fanout", durable: true);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName,"logs","");

                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                var stopWatch = new Stopwatch(); ;
                var msgNumb = 0;
                consumer.Received += (model, ea) =>
                {
                    if (msgNumb == 0)
                        stopWatch.Start();
                    msgNumb++;
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (msgNumb == expectedNumberOfmessages)
                    {
                        Console.WriteLine(msgNumb + " Received {0}", message);
                        stopWatch.Stop();
                        // Get the elapsed time as a TimeSpan value.
                        TimeSpan ts = stopWatch.Elapsed;

                        // Format and display the TimeSpan value.
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds);
                        Console.WriteLine("RunTime " + elapsedTime);
                    }
                    else
                    {
                        Console.WriteLine(msgNumb + " Received {0}", message);
                    }
                };
                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}