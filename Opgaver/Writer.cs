using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Opgaver;

public class Writer
{
    private string flightNo;
    private string eta;

    public Writer(string flightNo, string eta)
    {
        FlightNo = flightNo;
        ETA = eta;
    }

    public void Run()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "ExcelQueue2",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        string flightNo = FlightNo;
        string eta = ETA;

        var message = new { this.flightNo, this.eta };

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        
        channel.BasicPublish(exchange: "",
            routingKey: "ExcelQueue2",
            basicProperties: null,
            body: body
            );
    }
    public string FlightNo { get; set; }
    public string ETA { get; set; }
}