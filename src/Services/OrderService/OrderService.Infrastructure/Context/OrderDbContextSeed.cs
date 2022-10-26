using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.SeedWork;
using Polly;
using Polly.Retry;

namespace OrderService.Infrastructure.Context;

public class OrderDbContextSeed
    {
        public async Task SeedAsync(OrderDbContext context, ILogger<OrderDbContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderDbContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = false;

                var contentRootPath = "Seeding/Setup";


                using (context)
                {
                    context.Database.Migrate();

                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(useCustomizationData
                                                ? GetCardTypesFromFile(contentRootPath, logger)
                                                : GetPredefinedCardTypes());

                        await context.SaveChangesAsync();
                    }

                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(useCustomizationData
                                                ? GetOrderStatusFromFile(contentRootPath, logger)
                                                : GetPredefinedOrderStatus());
                    }

                    await context.SaveChangesAsync();
                }
            });
        }

        private IEnumerable<CardType> GetCardTypesFromFile(string contentRootPath, ILogger<OrderDbContextSeed> log)
        {
            string fileName = "CardTypes.txt";

            if (!File.Exists(fileName))
            {
                return GetPredefinedCardTypes();
            }

            int id = 1;
            return File.ReadAllLines(fileName)
                                        .Skip(1) // skip header column
                                        .Select(x => new CardType(id++, x))
                                        .Where(x => x != null);
        }

        private IEnumerable<CardType> GetPredefinedCardTypes()
        {
            return Enumeration.GetAll<CardType>();
        }

        private IEnumerable<OrderStatus> GetOrderStatusFromFile(string contentRootPath, ILogger<OrderDbContextSeed> log)
        {
            string fileName = "OrderStatus.csv";

            if (!File.Exists(fileName))
            {
                return GetPredefinedOrderStatus();
            }

            int id = 1;
            return File.ReadAllLines(fileName)
                                        .Skip(1) // skip header row
                                        .Select(x => new OrderStatus(id++, x))
                                        .Where(x => x != null);
        }

        private IEnumerable<OrderStatus> GetPredefinedOrderStatus()
        {
            return new List<OrderStatus>()
        {
            OrderStatus.Submitted,
            OrderStatus.AwaitingValidation,
            OrderStatus.StockConfirmed,
            OrderStatus.Paid,
            OrderStatus.Shipped,
            OrderStatus.Cancelled
        };
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<OrderDbContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }