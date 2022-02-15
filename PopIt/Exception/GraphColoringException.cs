namespace PopIt.Exception;
/// <summary>
/// An exception that should be thrown if graph coloring is not possible in some way.
/// </summary>
/// <remarks>This class cannot be inherited from</remarks>
public sealed class GraphColoringException : System.Exception
{
    public GraphColoringException(string message) : base($"Error while trying to color graph. Reason: '{message}'") { }
}

