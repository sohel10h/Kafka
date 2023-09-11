using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer.EventHandler
{
    public class OrderStatusUpdate : BackgroundService
    {
        private readonly ILogger<OrderStatusUpdate> _logger;
        private readonly ConsumerConfig _consumerConfig;
        private readonly string _topic;

         OrderStatusUpdate(
            ILogger<OrderStatusUpdate> logger,
            ConsumerConfig consumerConfig,
            string topic)
        {
            _logger = logger;
            _consumerConfig = consumerConfig;
            _topic = "order_status_updated";
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
                       .Build();

            consumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    if (consumeResult != null)
                    {
                        _logger.LogInformation($"Received message: {consumeResult.Message.Value}");
                        // Process the message here
                    }
                }
                catch (OperationCanceledException)
                {
                    // Graceful shutdown on cancellation
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error consuming message: {ex.Message}");
                }
            }
        }
    }
}
