namespace backend.Exceptions;

public class InvalidStepCountException : Exception
{
    public InvalidStepCountException()
        : base("Steps must be greater than zero.")
    {
    }
}
