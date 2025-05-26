namespace ISKI.OpcUa.Client.Exceptions;

public class OpcServiceException(string message, Exception? inner = null) : Exception(message, inner)
{
}
