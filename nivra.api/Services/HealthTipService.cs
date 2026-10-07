using backend.Repositories;
using backend.Models;

namespace backend.Services
{
    public class HealthTipService
    {
        private readonly HealthTipRepo _repository;

        public HealthTipService(HealthTipRepo repository)
        {
            _repository = repository;
        }

        private void ValidateIsNotEmpty(int n)
        {
            if(n <= 0)
                throw new ArgumentException("Šiuo metu sveikatos patarimų nėra");
        }

        public HealthTip GetRandomTip()
        {
            var tips = _repository.GetAll();
            var tipsList = tips.ToList();
            var count = tipsList.Count();
            ValidateIsNotEmpty(count);

            var random = new Random();
            int randomInt = random.Next(0, count);

            return tipsList[randomInt];
        }
    }
}