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
        using var channelSWA = connection.CreateModel();
        
        channelSWA.QueueDeclare(queue: "AICtoKLM",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        Console.WriteLine(" [*] Waiting for messages.");
        channelSWA.BasicQos(0, 0, true);
        var consumerKLM = new EventingBasicConsumer(channelSWA);
        consumerKLM.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
        };
        
        // channelSWA.BasicConsume(queue: "AICtoSWA",
        //     autoAck: true,
        //     consumer: consumerSWA);
        

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();

    }
}