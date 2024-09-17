namespace AIC_Receive;

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Receive
{
    static void Main(String[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channelSAS = connection.CreateModel();
        using var channelSWA = connection.CreateModel();

        channelSAS.QueueDeclare(queue: "SAStoAIRPORT",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        channelSWA.QueueDeclare(queue: "SWAtoAIRPORT",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        Console.WriteLine(" [*] Waiting for messages.");
        channelSAS.BasicQos(0, 0, true);
        var consumerSAS = new EventingBasicConsumer(channelSAS);
        consumerSAS.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
        };

        var consumerSWA = new EventingBasicConsumer(channelSWA);
        consumerSWA.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
        };
        
        channelSAS.BasicConsume(queue: "SAStoAIRPORT",
            autoAck: true,
            consumer: consumerSAS);
        
        channelSWA.BasicConsume(queue: "SWAtoAIRPORT",
            autoAck: true,
            consumer: consumerSWA);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();

    }
}    