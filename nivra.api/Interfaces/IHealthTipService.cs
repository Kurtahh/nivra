using backend.Models;

namespace backend.Interfaces;

public interface IHealthTipService
{
    HealthTip GetRandomTip();
}
