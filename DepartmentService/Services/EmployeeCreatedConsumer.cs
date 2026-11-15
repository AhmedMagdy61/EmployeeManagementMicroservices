using DepartmentService.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;

namespace DepartmentService.Services;

public class EmployeeCreatedConsumer : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;

    private const string QueueName = "employee_created";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var employee = JsonSerializer.Deserialize<EmployeeCreatedEvent>(message);

                    if (employee != null)
                    {
                        Console.WriteLine("New Employee Received!");
                        Console.WriteLine($"ID: {employee.Id}");
                        Console.WriteLine($"Name: {employee.Name}");
                        Console.WriteLine($"Email: {employee.Email}");
                        Console.WriteLine($"Department ID: {employee.DepartmentId}");
                        Console.WriteLine("-------------------------");
                    }

                    await _channel.BasicAckAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false
                    );
                };

                await _channel.BasicConsumeAsync(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer
                );

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitMQ Connection Error: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.DisposeAsync();

        if (_connection != null)
            await _connection.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}