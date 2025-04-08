using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Domain.Entities; // Assuming you have a namespace for your entities
using ShipAndThread.Domain.Enums; // Assuming you have a namespace for your enums

namespace ShipAndThread.Application.Services;

public class DatabaseQueueService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentQueue<Func<IServiceProvider, Task>> _writeQueue = new();
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public DatabaseQueueService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        StartProcessingQueue();
    }

    // ============================
    // Initialization and Lifecycle
    // ============================

    private void StartProcessingQueue()
    {
        Task.Run(async () =>
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (_writeQueue.TryDequeue(out var writeOperation))
                {
                    await _writeSemaphore.WaitAsync();
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        await writeOperation(scope.ServiceProvider);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing write operation: {ex.Message}");
                    }
                    finally
                    {
                        _writeSemaphore.Release();
                    }
                }
                else
                {
                    await Task.Delay(50); // Avoid busy-waiting
                }
            }
        });
    }

    public void StopProcessingQueue()
    {
        _cancellationTokenSource.Cancel();
    }

    // ============================
    // Write Operations
    // ============================

    public void EnqueueWrite(Func<IServiceProvider, Task> writeOperation)
    {
        _writeQueue.Enqueue(writeOperation);
    }

    public int GetWriteQueueSize()
    {
        return _writeQueue.Count;
    }

    // ============================
    // Read Operations
    // ============================

    public async Task<T> ReadAsync<T>(Func<IServiceProvider, Task<T>> readOperation)
    {
        using var scope = _serviceProvider.CreateScope();
        return await readOperation(scope.ServiceProvider);
    }
}