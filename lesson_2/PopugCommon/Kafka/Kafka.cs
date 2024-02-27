using Confluent.Kafka;
using System;
using System.Net;

namespace PopugCommon.Kafka
{
    public static class Kafka
    {
        public static void Produce(string topic, string key, string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
                //BootstrapServers = "host.docker.internal:9092",
                ClientId = Dns.GetHostName(),
                Acks = Acks.Leader,
                MessageTimeoutMs = 30000,
                MessageMaxBytes = 16777215
            };

            using (var producer = new ProducerBuilder<string, string>(config)
                    .SetKeySerializer(new StringSerializer())
                    .SetValueSerializer(new StringSerializer())
                    .Build())
            {
                try
                {
                    producer.Produce(
                        topic,
                        new Message<string, string>
                        {
                            Key = key,
                            Value = message
                        },
                        d =>
                        {
                            if (d.Error.IsError)
                                throw new InvalidOperationException(
                                    $"{d.Error.Code}:{d.Error.Reason}");
                        }
                        );


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
