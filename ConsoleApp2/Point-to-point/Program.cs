using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Point_to_point
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
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
                        FlightNr = "KL12345",
                        Airline = "KLM",
                        ScheduledArrivalTime = DateTime.UtcNow.AddHours(2).ToString("HH:mm:ss"),
                        EstimatedArrivalTime = DateTime.UtcNow.AddHours(2).AddMinutes(30).ToString("HH:mm:ss"),
                        Origin = "Amsterdam",
                        Destination = "Bluff City"
                    }
                };
                
                var etaMessage1 = new
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
                        FlightNr = "SA335",
                        Airline = "SAS",
                        ScheduledArrivalTime = DateTime.UtcNow.AddHours(2).ToString("HH:mm:ss"),
                        EstimatedArrivalTime = DateTime.UtcNow.AddHours(2).AddMinutes(30).ToString("HH:mm:ss"),
                        Origin = "London",
                        Destination = "Bluff City"
                    }
                };
                
                var etaMessage2 = new
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
                        FlightNr = "SW9323",
                        Airline = "SW",
                        ScheduledArrivalTime = DateTime.UtcNow.AddHours(2).ToString("HH:mm:ss"),
                        EstimatedArrivalTime = DateTime.UtcNow.AddHours(2).AddMinutes(30).ToString("HH:mm:ss"),
                        Origin = "Paris",
                        Destination = "Bluff City"
                    }
                };

                var airportTrafficControl = new Producer("ETAQueue", connection);
                airportTrafficControl.SendMessage(etaMessage);
                airportTrafficControl.SendMessage(etaMessage1);
                airportTrafficControl.SendMessage(etaMessage2);

                var airportInformationCenter = new Consumer("ETAQueue", connection);
                await airportInformationCenter.ReceiveMessages();
            }
        }
    }
}