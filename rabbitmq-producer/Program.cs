using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace rabbitmq_producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Run(async () => {
                var count = 1;
                while (true)
                {
                    try
                    {
                        Console.WriteLine("[x] Waiting {0} seconds...", 10);
                        await Task.Delay(TimeSpan.FromSeconds(10));

                        var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5671 };
                        factory.AuthMechanisms.Add(new ExternalMechanismFactory());
                        factory.Ssl.Enabled = true;
                        factory.Ssl.ServerName = "rabbitmq";
                        factory.Ssl.CertPath = "producer.p12";
                        factory.Ssl.CertPassphrase = "Besiege27pin67stoic";
                        factory.Ssl.CertificateValidationCallback += (sender, certificate, srvChain, sslPolicyErrors) =>
                        {
                            return VerifyServerCertificate(certificate);
                        };

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
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            });
        }

        private static bool VerifyServerCertificate(X509Certificate certificate)
        {
            var rootCa = new X509Certificate2("rootCA.crt");
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.ExtraStore.Add(rootCa);
            chain.Build(new X509Certificate2(certificate));
            string dump = Dump(chain.ChainStatus);
            Console.WriteLine($"Certificate validation status: { dump }");
            if (chain.ChainStatus.Length == 1 &&
                chain.ChainStatus.First().Status == X509ChainStatusFlags.UntrustedRoot)
            {
                Console.WriteLine($"chain is valid, thus cert signed by root certificate " +
                    $"and we expect that root is untrusted which the status flag tells us: { dump }");
                return true;
            }
            else
            {
                Console.WriteLine($"Certificate not valid, reasons: { dump }");
                // not valid for one or more reasons
                return false;
            }
        }

        private static string Dump(X509ChainStatus[] chainStatus)
        {
            var strBuilder = new StringBuilder();
            foreach (var status in chainStatus)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"{chainStatus.GetType().Name}");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append("{");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"Status : { status.Status },");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append($"StatusInformation : { status.StatusInformation },");
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append("}");
            }

            return strBuilder.ToString();
        }
    }
}
