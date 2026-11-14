using System.Diagnostics.CodeAnalysis;
using backend.Models;

namespace backend.Interfaces;

public interface IBackgroundTaskQueue
{
    Task Enqueue(Chat message);
    Task Dequeue([NotNullWhen(true )] out Chat? message);
    
}