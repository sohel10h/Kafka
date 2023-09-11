using Confluent.Kafka;
using Consumer.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer.EventHandler
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConsumer<string, string> consumer;
        private readonly ILogger<KafkaConsumerService> logger;

        public KafkaConsumerService(IOptions<KafkaConsumerConfig> consumerConfig, ILogger<KafkaConsumerService> logger)
        {
            this.logger = logger;
            var config = new ConsumerConfig
            {
                BootstrapServers = consumerConfig.Value.BootstrapServers,
                GroupId = consumerConfig.Value.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true // You can adjust this based on your needs
            };

            consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            consumer.Subscribe("order_status_updated"); // Replace with your Kafka topic name

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    if (consumeResult.IsPartitionEOF)
                    {
                        logger.LogInformation($"Reached end of partition: {consumeResult.Topic}/{consumeResult.Partition}");
                    }
                    else
                    {
                        logger.LogInformation($"Received message: {consumeResult.Message.Value}");
                        // Process the Kafka message as needed
                    }
                }
                catch (OperationCanceledException)
                {
                    // This exception is expected when stopping the service.
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error consuming Kafka message: {ex.Message}");
                }
            }
        }

        public override void Dispose()
        {
            consumer.Close();
            consumer.Dispose();
            base.Dispose();
        }




    }
}
