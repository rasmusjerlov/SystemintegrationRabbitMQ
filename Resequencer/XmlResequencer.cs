using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

List<dynamic> messages = new List<dynamic>();
var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();
var channelReseq = connection.CreateModel();

var xmlConsumer = new EventingBasicConsumer(channelReseq);
xmlConsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    dynamic deserializedMessage = JsonConvert.DeserializeObject(message);
    messages.Add(deserializedMessage);
    Console.WriteLine($" [x] Received {message}");
};


channelReseq.BasicConsume(queue: "XmlQueue",
    autoAck: true,
    consumer: xmlConsumer);

Thread.Sleep(5000);

messages.Sort((x, y) => x.SequenceNumber.CompareTo(y.SequenceNumber));

foreach (var msg in messages)
{
    
    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
    channelReseq.BasicPublish(exchange: "",
        routingKey: "XmlQueue",
        mandatory: false,
        basicProperties: null,
        body: body);
}

Console.WriteLine("Resequenced messages has been sent to the queue and have been consumed.");
Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();