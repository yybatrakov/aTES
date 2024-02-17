using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using System.Net;
using System.Text.Json;
using Mxm.Kafka;
using KafkaFlow;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Acks = Confluent.Kafka.Acks;

namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TasksRepository tasksRepository;

        public TasksController(TasksRepository tasksRepository)
        {
            this.tasksRepository = tasksRepository;
        }

        /// <summary>
        /// Получить таски
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "Tasks")]
        public List<tasks.Api.Data.Task> Get()
        {
            return tasksRepository.GetTasks();


        }
        /// <summary>
        /// Добавить таск
        /// </summary>
        /// <returns></returns>
        [HttpPost(Name = "Tasks")]
        public tasks.Api.Data.Task Post(tasks.Api.Data.Task task)
        {
            task = tasksRepository.AddTask(task);
            ProduceTask(task);
            return task;
        }

        private void ProduceTask(tasks.Api.Data.Task task)
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
                        "Tasks",
                        new Message<string, string>
                        {
                            Key = task.TaskId.ToString(),
                            Value = task.ToJson()
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