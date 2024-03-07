using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace PopugCommon.Kafka
{

    public abstract class KafkaConsumer 
    {
        public abstract string MessageType { get; }

        public abstract Task OnMessage(Message<Ignore, string> message);

        public async Task Consume(CancellationToken cancellationToken = default)
        {
             var config = new ConsumerConfig
             {
                 BootstrapServers = "kafka:9092",
                 GroupId = GetType().FullName,
                 AutoOffsetReset = AutoOffsetReset.Earliest,
                 AllowAutoCreateTopics = true,
                 EnableAutoCommit = false
             };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(MessageType);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        await OnMessage(consumeResult.Message);
                        consumer.Commit(consumeResult);
                    }
                    catch (Exception ex) { 
                    }
                }


                consumer.Close();
            }

        }
    }
}