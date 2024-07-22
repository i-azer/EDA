using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace EDA.Consumer
{
    internal interface IConsumerService
    {
        Task Receive();
    }

    internal class ConsumerService : BackgroundService, IConsumerService
    {
        private readonly IConsumer<int, string> _consumer;
        private readonly string _topicName;
        public ConsumerService(ConsumerConfig config, string topic)
        {
            _consumer = new ConsumerBuilder<int, string>(config).Build();
            _topicName = topic;
        }
        public async Task Receive()
        {
            await ExecuteAsync(CancellationToken.None);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _consumer.Subscribe(_topicName);
                try
                {
                    Console.WriteLine($"Trying to consume events on topic '{_topicName}'...");
                    var result = _consumer.Consume(stoppingToken);
                    await new MessageReceivedEventHandler().Handle(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to consume events on topic '{_topicName}': {ex.Message}");
                    Thread.Sleep(10000);
                }
            }
        }
    }
}
