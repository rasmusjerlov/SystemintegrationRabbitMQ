using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text.Json.Nodes;


class Send
{
    static void Main(String[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.ExchangeDeclare(exchange: "AICtoSAS", ExchangeType.Direct);
        channel.ExchangeDeclare(exchange: "AICtoKLM", ExchangeType.Direct);
        channel.ExchangeDeclare(exchange: "AICtoSWA", ExchangeType.Direct);
        
        channel.QueueDeclare(queue: "AICtoSAS",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        channel.QueueDeclare(queue: "AICtoKLM",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
            
        channel.QueueDeclare(queue: "AICtoSWA",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        channel.QueueBind(queue: "AICtoSAS", exchange: "AICtoSAS", routingKey: "SAS");
        channel.QueueBind(queue: "AICtoKLM", exchange: "AICtoKLM", routingKey: "KLM");
        channel.QueueBind(queue: "AICtoSWA", exchange: "AICtoSWA", routingKey: "SWA");

        var jsonFiles = new Dictionary<string, string>
        {
            {
                "/Users/rasmusjerlov/Library/Application Support/JetBrains/Rider2024.2/scratches/AIC_SendToSAS.json",
                "SAS"
            },
            {
                "/Users/rasmusjerlov/Library/Application Support/JetBrains/Rider2024.2/scratches/AIC_SendToKLM.json",
                "KLM"
            },
            {
                "/Users/rasmusjerlov/Library/Application Support/JetBrains/Rider2024.2/scratches/AIC_SendToSWA.json",
                "SWA"
            }
        };
        foreach (var kvp in jsonFiles)
        {
            string jsonFilePath = kvp.Key;
            string routingKey = kvp.Value;

            string jsonString = File.ReadAllText(jsonFilePath);
            var jsonObject = JsonSerializer.Deserialize<JsonDocument>(jsonString);
            string message = jsonObject.RootElement.ToString();
            var body = Encoding.UTF8.GetBytes(message);

            if (routingKey == "SAS")
            {
                channel.BasicPublish(exchange: "AICtoSAS",
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
            }
            if (routingKey == "KLM")
            {
                channel.BasicPublish(exchange: "AICtoKLM",
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
            }

            if (routingKey == "SWA")
            {
                channel.BasicPublish(exchange: "AICtoSWA",
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
            }

            Console.WriteLine($"Sent {message} to {routingKey}");
            
        }

        Console.WriteLine("Press [enter] to Exit.");
        Console.ReadLine();

    }
}