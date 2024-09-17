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

            channel.QueueDeclare(queue: "SAStoAIRPORT",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            
            string jsonFilePath =
                "/Users/rasmusjerlov/Library/Application Support/JetBrains/Rider2024.2/scratches/SAS.json";
            string jsonString = File.ReadAllText(jsonFilePath);

            var jsonObject = JsonSerializer.Deserialize<JsonDocument>(jsonString);

            string message = jsonObject.RootElement.ToString();
            
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                routingKey: "SAStoAIRPORT",
                basicProperties: null,
                body: body);

            Console.WriteLine($" [x] Sent {message}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }
    }    