using Confluent.Kafka;
using PopugCommon.KafkaMessages;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PopugCommon.Kafka
{
    public static class Kafka
    {
        public async static Task Produce(string topic, string key, PopugMessage message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
                ClientId = Dns.GetHostName(),
                Acks = Acks.All,
                MessageTimeoutMs = 30000,
                MessageMaxBytes = 16777215
            };

            
            var isValid = await message.Validate();
            if (!isValid)
                throw new Exception("JSON Schema invalid");

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
                            Value = message.ToJson()
                        },
                        d =>
                        {
                            if (d.Error.IsError)
                                throw new InvalidOperationException(
                                    $"{d.Error.Code}:{d.Error.Reason}");
                        }
                        );
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
