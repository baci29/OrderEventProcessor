using System.Data;
using Dapper;

public class OrderService : IOrderService
{
    private readonly IDbConnection _dbConnection;

    public OrderService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task ProcessOrderEvent(OrderEvent orderEvent)
    {
        var query = "INSERT INTO Orders (Id, Product, Total, Currency) VALUES (@Id, @Product, @Total, @Currency)";
        await _dbConnection.ExecuteAsync(query, orderEvent);
    }
}
