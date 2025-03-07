using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

public class RabbitMqListener : IHostedService
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMqListener(IOrderService orderService, IPaymentService paymentService)
    {
        _orderService = orderService;
        _paymentService = paymentService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "order_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "payment_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var msgType = (ea.BasicProperties.Headers["X-MsgType"] as byte[] != null) ? Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["X-MsgType"]) : string.Empty;

            if (msgType == "OrderEvent")
            {
                var orderEvent = JsonConvert.DeserializeObject<OrderEvent>(message);
                await _orderService.ProcessOrderEvent(orderEvent);
            }
            else if (msgType == "PaymentEvent")
            {
                var paymentEvent = JsonConvert.DeserializeObject<PaymentEvent>(message);
                await _paymentService.ProcessPaymentEvent(paymentEvent);
            }
        };

        _channel.BasicConsume(queue: "order_queue", autoAck: true, consumer: consumer);
        _channel.BasicConsume(queue: "payment_queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close();
        return Task.CompletedTask;
    }
}
