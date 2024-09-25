using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;

namespace ETA_Messages
{
    public class Producer
    {
        private readonly string _exchangeName;
        private readonly IConnection _connection;

        public Producer(string exchangeName, IConnection connection)
        {
            _exchangeName = exchangeName;
            _connection = connection;
        }

        public void SendMessage(object message)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: _exchangeName, type: "fanout");

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish(exchange: _exchangeName,
                    routingKey: "",
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" [x] Sent {0}", JsonConvert.SerializeObject(message));
            }
        }
    }
}