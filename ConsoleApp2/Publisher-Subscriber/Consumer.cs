
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ETA_Messages
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
                channel.QueueDeclare(queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var deserializedMessage = JsonConvert.DeserializeObject<dynamic>(message);
                    var formattedMessage = JsonConvert.SerializeObject(deserializedMessage, Formatting.Indented);
                    Console.WriteLine(" [x] Received:\n{0}", formattedMessage);
                };
                channel.BasicConsume(queue: _queueName,
                    autoAck: true,
                    consumer: consumer);
            }
        }
    }
}