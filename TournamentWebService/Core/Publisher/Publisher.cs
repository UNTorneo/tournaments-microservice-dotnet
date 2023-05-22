using System.Text;
using RabbitMQ.Client;

namespace TournamentWebService.Core.Publisher
{
    public class Publisher
    {
        public static void publishMessage(string url, string queue, string msg)
        {
            var factory = new ConnectionFactory { Uri = new Uri(url) };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = msg;
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: queue,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine($" [x] Sent data: {message}");
        }
    }
}
