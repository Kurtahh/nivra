using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using backend.Interfaces;
using backend.Models;

namespace backend.DataStructures;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly ConcurrentQueue<Chat> _queue = new();
    
    public void Enqueue(Chat message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _queue.Enqueue(message);
    }

    public bool TryDequeue([NotNullWhen(true)] out Chat? message)
    {
        return _queue.TryDequeue(out message) ;
    }
}