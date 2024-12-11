using Silk.NET.GLFW;

namespace VeynEngine;

/// <summary>
/// The glfw exception is the excpetion thrown when an error occurs in the underlying glfw library.
/// </summary>
public sealed class VeynGlfwException(ErrorCode error) : Exception
{
    /// <summary>
    /// The GLFW error code.
    /// </summary>
    public ErrorCode ErrorCode { get; } = error;
}