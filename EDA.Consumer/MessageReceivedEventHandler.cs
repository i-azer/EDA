using Confluent.Kafka;

namespace EDA.Consumer
{
    internal class MessageReceivedEventHandler
    {
        public async Task Handle(ConsumeResult<int, string> result)
        {
            await Task.Run(() => { Console.WriteLine(result.Message.Value); });
        }
    }
}