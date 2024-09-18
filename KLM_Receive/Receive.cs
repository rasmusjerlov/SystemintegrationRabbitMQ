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
        using var channelKLM = connection.CreateModel();
        
        channelKLM.QueueDeclare(queue: "AICtoKLM",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        Console.WriteLine(" [*] Waiting for messages.");
        channelKLM.BasicQos(0, 0, true);
        var consumerKLM = new EventingBasicConsumer(channelKLM);
        consumerKLM.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
        };
        
        // channelKLM.BasicConsume(queue: "AICtoKLM",
        //     autoAck: true,
        //     consumer: consumerKLM);
        

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();

    }
}