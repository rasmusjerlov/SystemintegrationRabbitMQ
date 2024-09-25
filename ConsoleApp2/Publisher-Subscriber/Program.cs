using System;
using System.Threading.Tasks;
using ETA_Messages;
using RabbitMQ.Client;

namespace Systemintegration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Enter Flight Information:");

            Console.Write("Flight Number: ");
            string flightNo = Console.ReadLine();

            Console.Write("Airline: ");
            string airline = Console.ReadLine();

            Console.Write("Scheduled Arrival Time (HH:mm:ss): ");
            string scheduledTimeInput = Console.ReadLine();
            TimeSpan scheduledTime = TimeSpan.Parse(scheduledTimeInput);

            TimeSpan estimatedTime = scheduledTime.Add(TimeSpan.FromMinutes(30));

            string scheduledArrivalTime = DateTime.UtcNow.Date.Add(scheduledTime).ToString("dd-MMMM-yyyy HH:mm:ss");
            string estimatedArrivalTime = DateTime.UtcNow.Date.Add(estimatedTime).ToString("dd-MMMM-yyyy HH:mm:ss");

            Console.Write("Origin: ");
            string origin = Console.ReadLine();

            Console.Write("Destination: ");
            string destination = Console.ReadLine();

            var etaMessage = new
            {
                Header = new
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    Sender = "Air Traffic Control",
                    Receiver = "Airport Information Center"
                },
                Body = new
                {
                    FlightNo = flightNo,
                    Airline = airline,
                    ScheduledArrivalTime = scheduledArrivalTime,
                    EstimatedArrivalTime = estimatedArrivalTime,
                    Origin = origin,
                    Destination = destination
                }
            };

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                var producer = new Producer("ETAExchange", connection);
                producer.SendMessage(etaMessage);

                var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "SASQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: "SWAQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue: "KLMQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                channel.QueueBind(queue: "SASQueue", exchange: "ETAExchange", routingKey: "");
                channel.QueueBind(queue: "SWAQueue", exchange: "ETAExchange", routingKey: "");
                channel.QueueBind(queue: "KLMQueue", exchange: "ETAExchange", routingKey: "");

                var consumerSAS = new Consumer("SASQueue", connection);
                var consumerSWA = new Consumer("SWAQueue", connection);
                var consumerKLM = new Consumer("KLMQueue", connection);

                //await Task.WhenAll(consumerSAS.ReceiveMessages(), consumerSWA.ReceiveMessages(), consumerKLM.ReceiveMessages());
            }
        }
    }
}