using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost"};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "XmlAggregatorQueue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);
    
Thread.Sleep(5000);

var xmlConsumer = new EventingBasicConsumer(channel);
xmlConsumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
};

channel.BasicConsume(queue: "XmlAggregatorQueue",
    autoAck: true,
    consumer: xmlConsumer);
