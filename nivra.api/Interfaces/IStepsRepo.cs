using backend.Models;

namespace backend.Interfaces;

public interface IStepsRepo
{
    StepEntry? EntryByDate(DateOnly date, long userId);
    void Add(StepEntry entry);
    void Update(StepEntry entry);
}
