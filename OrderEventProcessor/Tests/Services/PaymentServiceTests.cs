using Xunit;
using Moq;
using System.Data;
using Dapper;
using RabbitMQ.Client;
using OrderEventProcessor.Services;
using OrderEventProcessor.Models;
using Newtonsoft.Json;
using System.Text;

public class PaymentServiceTests
{
    [Fact]
    public async Task ProcessPaymentEvent_OrderNotFound_ShouldRepublishPaymentEvent()
    {
        var dbConnectionMock = new Mock<IDbConnection>();
        var channelMock = new Mock<IModel>();
        var paymentService = new PaymentService(dbConnectionMock.Object, channelMock.Object);
        var paymentEvent = new PaymentEvent { OrderId = "O-123", Amount = 11.00m };

        await paymentService.ProcessPaymentEvent(paymentEvent);

        channelMock.Verify(ch => ch.BasicPublish(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IBasicProperties>(),
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == JsonConvert.SerializeObject(paymentEvent))),
            Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentEvent_OrderFound_ShouldPrintStatus()
    {
        var dbConnectionMock = new Mock<IDbConnection>();
        dbConnectionMock.Setup(db => db.QueryFirstOrDefaultAsync<OrderEvent>(
            It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new OrderEvent { Id = "O-123", Product = "PR-ABC", Total = 12.34m, Currency = "USD" });

        var channelMock = new Mock<IModel>();
        var paymentService = new PaymentService(dbConnectionMock.Object, channelMock.Object);
        var paymentEvent = new PaymentEvent { OrderId = "O-123", Amount = 12.34m };

        var consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(consoleOutput));
        await paymentService.ProcessPaymentEvent(paymentEvent);

        var output = consoleOutput.ToString().Trim();
        Assert.Contains("Order: O-123, Product: PR-ABC, Total: 12.34 USD, Status: PAID", output);
    }
}
