using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Point_to_point
{
    public class Consumer
    {
        private readonly string _queueName;
        private readonly IConnection _connection;

        public Consumer(string queueName, IConnection connection)
        {
            _queueName = queueName;
            _connection = connection;
        }

        public async Task ReceiveMessages()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var etaMessage = JsonSerializer.Deserialize<object>(message);
                    Console.WriteLine($"Received message: {message}");
                };

                channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            }
        }
    }
}