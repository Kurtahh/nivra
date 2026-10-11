namespace backend.Exceptions;

public class InvalidTipCategoryException : Exception
{
    public string InvalidValue { get; }

    public InvalidTipCategoryException(string invalidValue, IEnumerable<string> validValues)
        : base($"'{invalidValue}' is not a valid tip category. Valid categories: {string.Join(", ", validValues)}")
    {
        InvalidValue = invalidValue;
    }
}