namespace CodeAssignmentTemplate.Exceptions;

/// <summary>
/// Thrown when SMHI returns a successful response but the payload contains no usable data.
/// Mapped to 502 Bad Gateway.
/// </summary>
public sealed class SmhiUnavailableException : Exception
{
    public SmhiUnavailableException(string message) : base(message) { }
}
