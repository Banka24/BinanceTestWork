using Binance.Net.Clients;
using BinanceTestWork.Core.Application;
using BinanceTestWork.Core.Application.Commands;
using BinanceTestWork.Core.Application.Validators;
using BinanceTestWork.Core.Domain.Services;
using BinanceTestWork.Infrastructure;
using CryptoExchange.Net.CommonObjects;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace BinanceTestWork.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console()
                    .MinimumLevel.Debug();
            });

            builder.Services.AddValidatorsFromAssemblyContaining<CheckStatusJobQueryValidator>(ServiceLifetime.Transient);

            builder.Services.Configure<BinanceApiSettings>(
                builder.Configuration.GetSection("BinanceApi")
            );

            builder.Services.AddSingleton(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<BinanceApiSettings>>().Value;
                return new BinanceRestClient(options =>
                {
                    options.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials(
                        settings.ApiKey,
                        settings.ApiSecret);
                });
            });

            builder.Services.AddScoped<IBinanceService<Kline>, BinanceService>();

            builder.Services.AddSingleton<IMongoClient>(_ =>
            {
                var connectionString = builder.Configuration.GetConnectionString("MongoDb");
                return new MongoClient(connectionString);
            });

            builder.Services.AddScoped<IRepository<Kline>>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<MongoDBRepository>>();
                var mongoClient = provider.GetRequiredService<IMongoClient>();
                var configuration = provider.GetRequiredService<IConfiguration>();

                // Чтение имени базы данных и коллекции из конфигурации
                var databaseName = configuration["MongoDb:DatabaseName"];
                var collectionName = configuration["MongoDb:CollectionName"];

                if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(collectionName))
                {
                    throw new InvalidOperationException("DatabaseName or CollectionName is not configured in appsettings.json.");
                }

                return new MongoDBRepository(logger, mongoClient, databaseName, collectionName);
            });

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<LoadJobCommandHandler>());

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.MapControllers();

            app.Run();
        }
    }
}