
using RabbitMQ.Client;
using EmployeeService.Messages;
using System.Text.Json;
using System.Text;

namespace EmployeeService.Services;

public class RabbitMQPublisher : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private const string QueueName = "employee_created";

    public async Task PublishEmployeeCreatedAsync(EmployeeCreatedEvent employee)
    {
        await _lock.WaitAsync();

        try
        {
            if (_channel == null)
            {
                var factory = new ConnectionFactory
                {
                    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
                    Port = 5672,
                    UserName = "guest",
                    Password = "guest"
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );
            }

            var message = JsonSerializer.Serialize(employee);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine("Employee Created Event Published!");
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.DisposeAsync();

        if (_connection != null)
            await _connection.DisposeAsync();

        _lock.Dispose();
    }
}
