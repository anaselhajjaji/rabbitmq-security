using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rabbitmq_consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5671, UserName = "rabbitmq", Password = "rabbitmq" };
                    factory.Ssl.Enabled = true;
                    factory.Ssl.ServerName = "rabbitmq";
                    factory.Ssl.CertPath = "consumer.p12";
                    factory.Ssl.CertPassphrase = "Besiege27pin67stoic";
                    factory.Ssl.CertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true; // TODO, to be implemented.

                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "hello",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);
                        };
                        channel.BasicConsume(queue: "hello",
                                             autoAck: true,
                                             consumer: consumer);

                        // Wait indefinitely
                        await Task.Delay(Timeout.Infinite);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Console.WriteLine("[x] Waiting {0} seconds...", 10);
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
