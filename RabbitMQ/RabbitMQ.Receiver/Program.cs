using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

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
                var data = new List<string>();
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
                        var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:00}";
                        Console.WriteLine("RunTime " + elapsedTime);
                        msgNumb = 0;
                        stopWatch = new Stopwatch();
                        data.Add(elapsedTime);
                        Task.Factory.StartNew(() => File.WriteAllText("./results.txt", JsonConvert.SerializeObject(data)));
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