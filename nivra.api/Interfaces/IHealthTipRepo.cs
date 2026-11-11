using backend.Models;

namespace backend.Interfaces;

public interface IHealthTipRepo
{
    IEnumerable<HealthTip> GetAll();
    void Add(HealthTip tip);
}
