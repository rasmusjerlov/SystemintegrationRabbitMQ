using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "XmlAggregatorQueue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var messages = new List<dynamic>();

var xmlConsumer = new EventingBasicConsumer(channel);
xmlConsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    dynamic deserializedMessage = JsonConvert.DeserializeObject(message);
    messages.Add(deserializedMessage);
    Console.WriteLine($" [x] Received {message}");
};

channel.BasicConsume(queue: "XmlAggregatorQueue",
    autoAck: true,
    consumer: xmlConsumer);

// Wait for a short period to allow messages to be received
Thread.Sleep(5000);

// Group messages by ReservationNumber
var groupedMessages = messages.GroupBy(m => m.ReservationNumber);

// Aggregate messages into one message per passenger
var aggregatedMessages = new List<dynamic>();
foreach (var group in groupedMessages)
{
    var flight = group.FirstOrDefault(m => m.Type == "Flight");
    var passenger = group.FirstOrDefault(m => m.FirstName == "Anders");
    var luggages = group.Where(m => m.Type == "Luggage").ToList();

    var aggregatedMessage = new
    {
        Flight = flight,
        Passenger = passenger,
        Luggages = luggages
    };

    aggregatedMessages.Add(aggregatedMessage);
}

// Publish aggregated messages to a new queue
channel.QueueDeclare(queue: "AggregatedQueue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

foreach (var msg in aggregatedMessages)
{
    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
    channel.BasicPublish(exchange: "",
        routingKey: "AggregatedQueue",
        mandatory: false,
        basicProperties: null,
        body: body);
}

Console.WriteLine("Aggregated messages have been sent to the queue.");
Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();