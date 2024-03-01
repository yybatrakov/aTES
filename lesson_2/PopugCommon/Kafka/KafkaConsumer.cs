using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using static Confluent.Kafka.ConfigPropertyNames;

namespace PopugCommon.Kafka
{

    public abstract class KafkaConsumer 
    {

        public KafkaConsumer()
        {
        }


        public abstract string MessageType { get; }

        private ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = "kafka:9092",
            GroupId = "foo",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true
        };

        public abstract Task OnMessage(Message<Ignore, string> message);

        public async Task Consume(CancellationToken cancellationToken = default)
        {

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
                    catch (Exception ex) { }
                }


                consumer.Close();
            }

        }
        //public async Task StartAsync(CancellationToken cancellationToken)
        //{
        //    await Task.Run(() => Consume(cancellationToken), cancellationToken);
        //    await Task.CompletedTask;
        //}

        //public async Task StopAsync(CancellationToken cancellationToken)
        //{
        //    await Task.CompletedTask;
        //}
    }
}