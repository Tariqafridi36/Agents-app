// Install the RabbitMQ.Client NuGet package

using RabbitMQ.Client;
using System.Text;

public class QueueManager
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public QueueManager()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "chat_sessions", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void EnqueueChatSession(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: "chat_sessions", basicProperties: null, body: body);
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}
