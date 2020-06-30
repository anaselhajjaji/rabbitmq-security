using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace rabbitmq_producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Run(async () => {
                var count = 1;
                while(true)
                {
                    Console.WriteLine("[x] Waiting {0} seconds...", 10);
                    await Task.Delay(TimeSpan.FromSeconds(10));

                    var factory = new ConnectionFactory() { HostName = "127.0.0.1", UserName = "rabbitmq", Password = "rabbitmq" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "hello",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        string message = $"Hello World number { count++ }";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "hello",
                                             basicProperties: null,
                                             body: body);
                        Console.WriteLine("[x] Sent {0}", message);
                    }
                }
            });
        }
    }
}
