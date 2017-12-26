using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace RabbitMQ.SenderTimed
{
    class Program
    {
        static void Main(string[] args)
        {
            var ip = "localhost";
            var expectedNumberOfmessages = 1000;
            var delay = TimeSpan.FromMilliseconds(3000);
            if (args.Length > 0)
            {
                ip = args.First();
                expectedNumberOfmessages = Int32.Parse(args[1]);
                delay = TimeSpan.FromMilliseconds(Int32.Parse(args.Last()));
            }
            Console.WriteLine("Running with IP: " + ip);
            var factory = new ConnectionFactory() { HostName = ip, UserName = "test", Password = "test" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout", durable: true);

                Console.WriteLine("Hello - ready?");
                Console.ReadLine();
                for (var i = 0; i < expectedNumberOfmessages; i++)
                {
                    Thread.Sleep(delay);
                    channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: Encoding.UTF8.GetBytes("Hello world from (Client 1) - " + i));
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
