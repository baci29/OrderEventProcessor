using Xunit;
using Moq;
using System.Data;
using Dapper;
using OrderEventProcessor.Services;
using OrderEventProcessor.Models;

public class OrderServiceTests
{
    [Fact]
    public async Task ProcessOrderEvent_ShouldInsertOrder()
    {
        var dbConnectionMock = new Mock<IDbConnection>();
        var orderService = new OrderService(dbConnectionMock.Object);
        var orderEvent = new OrderEvent { Id = "O-123", Product = "PR-ABC", Total = 12.34m, Currency = "USD" };

        await orderService.ProcessOrderEvent(orderEvent);

        dbConnectionMock.Verify(db => db.ExecuteAsync(It.IsAny<string>(), orderEvent), Times.Once);
    }
}
