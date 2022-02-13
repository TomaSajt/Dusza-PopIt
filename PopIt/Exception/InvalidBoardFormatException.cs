namespace PopIt.Exception;
public sealed class InvalidBoardFormatException : System.Exception
{
    public InvalidBoardFormatException(string message) : base($"Board was of the wrong format. Reason: '{message}'") { }
}

