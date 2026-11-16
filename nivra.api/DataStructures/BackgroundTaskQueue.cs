using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using backend.Interfaces;
using backend.Models;

namespace backend.DataStructures;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    readonly ConcurrentQueue<Chat> queue = new();
    
    
    public Task Enqueue(Chat message)
    {
        ArgumentsNullException.TrowIfNull(message);
        queue.Enqueue(message);
    }

    public Task TryDequeue([NotNullWhen(true)] out Chat? message)
    {
        return queue.TryDequeue(out message);
    }
}