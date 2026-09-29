using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace backend.Models
{
    public class StepEntry{
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int StepCount { get; set; }
        public int UserId { get; set; }
    }
}

