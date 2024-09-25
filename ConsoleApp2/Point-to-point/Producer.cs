using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Point_to_point
{
    public class Producer
    {
        private readonly string _queueName;
        private readonly IConnection _connection;

        public Producer(string queueName, IConnection connection)
        {
            _queueName = queueName;
            _connection = connection;
        }

        public void SendMessage(object message)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var messageBody = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageBody);

                channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            }
        }
    }
}