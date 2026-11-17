using System.Diagnostics.CodeAnalysis;
using backend.Models;

namespace backend.Interfaces;

public interface IBackgroundTaskQueue
{
    void Enqueue(Chat message);
    bool TryDequeue([NotNullWhen(true )] out Chat? message);
    
}