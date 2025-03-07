using System.Data;
using Dapper;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

public class PaymentService : IPaymentService
{
    private readonly IDbConnection _dbConnection;
    private readonly IModel _channel;

    public PaymentService(IDbConnection dbConnection, IModel channel)
    {
        _dbConnection = dbConnection;
        _channel = channel;
    }

    public async Task ProcessPaymentEvent(PaymentEvent paymentEvent)
    {
        var order = await _dbConnection.QueryFirstOrDefaultAsync<OrderEvent>(
            "SELECT * FROM Orders WHERE Id = @Id", new { Id = paymentEvent.OrderId });

        if (order == null)
        {
            // Uloží zprávu PaymentEvent zpět do RabbitMQ fronty, pokud objednávka neexistuje
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(paymentEvent));
            var properties = _channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object> { { "X-MsgType", "PaymentEvent" } };

            _channel.BasicPublish(exchange: "", routingKey: "payment_queue", basicProperties: properties, body: body);
        }
        else
        {
            // Zkontroluje stav platby a vypište zprávu do konzole
            if (order.Total <= paymentEvent.Amount)
            {
                Console.WriteLine($"Order: {order.Id}, Product: {order.Product}, Total: {order.Total} {order.Currency}, Status: PAID");
            }
            else
            {
                Console.WriteLine($"Order: {order.Id}, Product: {order.Product}, Total: {order.Total} {order.Currency}, Status: PARTIALLY PAID");
            }
        }
    }
}
