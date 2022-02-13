namespace PopIt.Exception;
sealed class InvalidBoardFormatException : System.Exception
{
    public InvalidBoardFormatException(string message) : base($"Board was of the wrong format. Reason: '{message}'") { }
}

