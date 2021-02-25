using System;
using System.Text;
using RabbitMQ.Client;

namespace ToursApp
{
    public class ToursApplication
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
            channel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "cancelled");

            Console.WriteLine("Booking someone.");
            
            const string bookingMessage = "I WOULD LIKE TO BOOK THIS TOUR";
            var body = Encoding.UTF8.GetBytes(bookingMessage);
            
            channel.BasicPublish(
                exchange:exchangeName,
                routingKey:"booked",
                basicProperties: null,
                body: body);
            
            Console.WriteLine("Press [enter] to cancel the tour again.");
            Console.ReadLine();
            
            
            const string cancellingMessage = "OH SHIT NO NEVER MIND, MIKE IS ON THIS TRIP!";
            var cancelBody = Encoding.UTF8.GetBytes(cancellingMessage);
            
            channel.BasicPublish(
                exchange:exchangeName,
                routingKey:"cancelled",
                basicProperties: null,
                body: cancelBody);
            
            Console.WriteLine("Cancelling a tour.");
            Console.ReadLine();

        }
    }
}
