using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PopugCommon.Kafka;

public class KafkaConsumersStartupService : IHostedService
{
    private readonly IEnumerable<KafkaConsumer> _consumers;
    private readonly List<Task> _tasks = new();
    private readonly CancellationTokenSource _cancellationTokenSource;

    public KafkaConsumersStartupService(
        IEnumerable<KafkaConsumer> consumers)
    {
        _consumers = consumers;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private async Task StartConsumer(KafkaConsumer consumer, CancellationToken cancellationToken)
    {
        try
        {
            await consumer.Consume(cancellationToken);
        }
        catch (Exception e)
        {
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var cancel = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

        foreach (var consumer in _consumers)
        {
            _tasks.Add(Task.Run(() => StartConsumer(consumer, cancel.Token), cancel.Token));
        }

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();

        Task.WaitAll(_tasks.ToArray(), cancellationToken);

        await Task.CompletedTask;
    }
}