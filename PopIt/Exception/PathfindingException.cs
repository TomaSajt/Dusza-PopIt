namespace PopIt.Exception;
/// <summary>
/// An exception that should be thrown when pathfinding fails.
/// </summary>
/// <remarks>This class cannot be inherited from</remarks>
public sealed class PathfindingException : System.Exception
{
    public PathfindingException() : base("Error while pathfinding.") { }
    public PathfindingException(string reason) : base($"Error while pathfinding. Reason: '{reason}'") { }
}

