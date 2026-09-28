namespace IPL_Franchises.Application.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message)
        : base(message)
    {
    }
}
