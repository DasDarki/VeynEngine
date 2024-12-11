namespace VeynEngine;

/// <summary>
/// The veyn engine exception is the base exception for all exceptions thrown by the Veyn engine.
/// </summary>
public class VeynEngineException(string? message = null, Exception? innerException = null) : Exception(message, innerException);