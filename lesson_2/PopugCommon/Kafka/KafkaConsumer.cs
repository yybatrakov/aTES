using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using PopugCommon.KafkaMessages;

namespace PopugCommon.Kafka
{

    public abstract class KafkaConsumer 
    {
        public abstract string MessageType { get; }

        public abstract Task OnMessage(PopugMessage popug);

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
                        var popug = consumeResult.Message.Value.FromJson<PopugMessage>();
                        if (!await popug.Validate())
                        {
                            await SaveInDlq(consumeResult.Message.Value);
                            return;
                        }
                        await OnMessage(popug);
                        consumer.Commit(consumeResult);
                    }
                    catch (Exception ex) { 
                    }
                }


                consumer.Close();
            }
        }

        public async Task SaveInDlq(string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
                ClientId = Dns.GetHostName(),
                Acks = Acks.All,
                MessageTimeoutMs = 30000,
                MessageMaxBytes = 16777215
            };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                try
                {
                    producer.Produce(
                        $"{GetType().FullName}-{MessageType}-dlq",
                        new Message<string, string>
                        {
                            Key = DateTime.Now.Ticks.ToString(),
                            Value = message
                        },
                        d =>
                        {
                            if (d.Error.IsError)
                                throw new InvalidOperationException(
                                    $"{d.Error.Code}:{d.Error.Reason}");
                        });
                    producer.Flush();
                }
                catch (ProduceException<string, string> e)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
    }
}