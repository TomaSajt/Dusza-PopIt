﻿namespace PopIt.Exception;
internal class GraphColoringException : System.Exception
{
    public GraphColoringException(string message) : base($"Error while trying to color graph. Reason: '${message}'") { }
}
