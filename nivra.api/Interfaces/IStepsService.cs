namespace backend.Interfaces;

public interface IStepsService
{
    void LogSteps(int stepCount, long userId);
    int GetTodaySteps(long id);
}
