
using Confluent.Kafka;

namespace EDA.Producer
{
    internal interface IProducerService
    {
        Task Send(string message);
        Task SetTopic(string topicName);
    }
    internal class ProducerService : BackgroundService, IProducerService
    {
        private readonly ProducerConfig _config = default!;
        private readonly ILogger<ProducerService> _logger = default!;
        private readonly IProducer<int, string> _producer = default!;
        private readonly ProducerBuilder<int, string> _builder = default!;
        public string Topic { get; private set; }
        public ProducerService(ProducerConfig config, ILogger<ProducerService> logger)
        {
            _config = config;
            _logger = logger;
            Topic = "";
            _builder = new ProducerBuilder<int, string>(_config);
            _producer = _builder.Build();
        }
        public async Task Send(string message)
        {
            await Task.Run(() =>
            {
                var messagePacket = new Message<int, string>() { Key = 1, Value = message };
                _producer.ProduceAsync(Topic, messagePacket, CancellationToken.None);

            });
        }

        public async Task SetTopic(string topicName)
        {
            await Task.Run(() => { Topic = topicName; });
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
