namespace notissimus.toyparser.core.Exceptions;

public class NetworkException : ApplicationException
{
    public NetworkException(string url) : base($"Can not reach {url.Trim()}")
    { }
}