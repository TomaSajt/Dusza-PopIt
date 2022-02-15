using PopIt.Data;
namespace PopIt.Exception;
/// <summary>
/// An exception that should be thrown when a <see cref="Board"/> is not of the format that is required by the program. This class cannot be inherited from.
/// </summary>
public sealed class InvalidBoardFormatException : System.Exception
{
    public InvalidBoardFormatException(string reason) : base($"Board was of the wrong format. Reason: '{reason}'") { }
}

