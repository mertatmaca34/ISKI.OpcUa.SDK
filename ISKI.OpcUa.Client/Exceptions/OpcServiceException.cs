namespace ISKI.OpcUa.Client.Exceptions;

public class OpcServiceException : Exception
{
    public OpcServiceException(string message, Exception? inner = null)
        : base(message, inner) { }
}
