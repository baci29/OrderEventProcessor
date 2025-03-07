using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Npgsql;
using RabbitMQ.Client;
using System.Data;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("Postgres");
                var rabbitMqHostName = context.Configuration["RabbitMq:HostName"];

                services.AddSingleton<IDbConnection>(sp => new NpgsqlConnection(connectionString));
                services.AddSingleton<IOrderService, OrderService>();
                services.AddSingleton<IPaymentService, PaymentService>();
                services.AddSingleton(sp =>
                {
                    var factory = new ConnectionFactory() { HostName = rabbitMqHostName };
                    var connection = factory.CreateConnection();
                    return connection.CreateModel();
                });
                services.AddHostedService<RabbitMqListener>();
            })
            .Build();

        await host.RunAsync();
    }
}
