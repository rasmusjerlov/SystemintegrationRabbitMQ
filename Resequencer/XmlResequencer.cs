using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MiniProjekt;

public class XmlResequencer
{
    static void Main(String[] args)
    {
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
            // Console.WriteLine($" [x] Received {message}");
        };
        

        channelReseq.BasicConsume(queue: "XmlQueue",
            autoAck: true,
            consumer: xmlConsumer);
        
        messages.Sort((x, y) => x.SequenceNumber.CompareTo(y.SequenceNumber));
        Console.WriteLine("Sorted:");
        foreach (var msg in messages)
        {
            Console.WriteLine($"Processed message with SequenceNumber: {msg.SequenceNumber}");
        }
        
        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }
}