using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailService
{
    public class EmailService
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            const string exchangeName = "topic_tour";
            channel.ExchangeDeclare(exchange: exchangeName, type: "topic");
            var queueName = channel.QueueDeclare().QueueName;
            
            channel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "booked");

            Console.WriteLine("Waiting for messages. To exit press enter!");
            
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(
                    "EmailService received '{0}':'{1}'",
                    routingKey,
                    message);
            };

            channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);

            Console.ReadLine();
        }
    }
}
