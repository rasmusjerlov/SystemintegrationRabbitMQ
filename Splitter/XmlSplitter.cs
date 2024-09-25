using System.Text;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MiniProjekt;

public class XmlSplitter
{
    static void Main(String[] args)
    {
        try
        {
            string xmlFilePath = "/Users/rasmusjerlov/Library/CloudStorage/OneDrive-EFIF/Datamatiker/Projects/Rider/SystemintegrationRabbitMQ/MiniProjekt/FlightDetalisInfoResponse.xml";
            var xmlContent = System.IO.File.ReadAllText(xmlFilePath);
            var xDocument = XDocument.Parse(xmlContent);

            var flightDetails = xDocument.Element("FlightDetailsInfoResponse").Element("Flight");
            var passenger = xDocument.Element("FlightDetailsInfoResponse").Element("Passenger");
            var luggages = xDocument.Element("FlightDetailsInfoResponse").Elements("Luggage");

            var messages = new List<string>();
            int sequenceNumber = 0;

            // Create flight details message
            var flightMessage = new
            {
                Type = "Flight",
                Number = flightDetails.Attribute("number").Value,
                FlightDate = flightDetails.Attribute("Flightdate").Value,
                Origin = flightDetails.Element("Origin").Value,
                Destination = flightDetails.Element("Destination").Value,
                Timestamp = DateTime.Now,
                SequenceNumber = sequenceNumber++
            };
            messages.Add(JsonConvert.SerializeObject(flightMessage));

            // Create passenger message
            var passengerMessage = new
            {
                Type = "Passenger",
                ReservationNumber = passenger.Element("ReservationNumber").Value,
                FirstName = passenger.Element("FirstName").Value,
                LastName = passenger.Element("LastName").Value,
                Timestamp = DateTime.Now,
                SequenceNumber = sequenceNumber++
            };
            messages.Add(JsonConvert.SerializeObject(passengerMessage));

            // Create luggage messages
            foreach (var luggage in luggages)
            {
                var luggageMessage = new
                {
                    Type = "Luggage",
                    Id = luggage.Element("Id").Value,
                    Identification = luggage.Element("Identification").Value,
                    Category = luggage.Element("Category").Value,
                    Weight = luggage.Element("Weight").Value,
                    SequenceNumber = sequenceNumber++
                };
                messages.Add(JsonConvert.SerializeObject(luggageMessage));
            }

            // Send messages to RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "XmlQueue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            foreach (var message in messages)
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                    routingKey: "XmlQueue",
                    basicProperties: null,
                    body: body);
            }

            Console.WriteLine("Messages sent to RabbitMQ.");
        }
        catch (NullReferenceException nr)
        {
            Console.WriteLine(nr.Message);
        }
    }
}