using System.Text.Json.Serialization;

namespace backend.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BlogCategory
    {
        Recipes,
        Fitness,
        Nutrition,
        HealthyHabits,
        PersonalExperience,
        Other
    }
}