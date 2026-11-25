namespace backend.Exceptions;

public class InvalidStepCountException : Exception
{
    public InvalidStepCountException(int minStepCount)
        : base("Steps must be greater than " + minStepCount.ToString())
    {
    }
}